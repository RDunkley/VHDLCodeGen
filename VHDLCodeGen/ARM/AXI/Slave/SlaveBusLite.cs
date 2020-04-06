//********************************************************************************************************************************
// Filename:    SlaveBusLite.cs
// Owner:       Richard Dunkley
//********************************************************************************************************************************
// Copyright © Richard Dunkley 2020
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0  Unless required by applicable
// law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
// limitations under the License.
//********************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace VHDLCodeGen.ARM.AXI.Slave
{
	/// <summary>
	///   Contains information about and generates VHDL for an AXI Lite bus.
	/// </summary>
	public class SlaveBusLite : BusLite
	{
		#region Properties

		/// <summary>
		///   Gets or sets the default register value for registers that don't have a default value specified.
		/// </summary>
		public ulong DefaultRegisterValue { get; set; }

		/// <summary>
		///   True if the bus has writeable blocks, false otherwise.
		/// </summary>
		public bool HasWriteableBlocks
		{
			get
			{
				return (Registers.WriteCount > 0 || Memories.WriteCount > 0 || Notifications.WriteCount > 0);
			}
		}

		/// <summary>
		///   True if the bus has writeable registers (notification or otherwise), false otherwise.
		/// </summary>
		public bool HasWriteableRegisters
		{
			get
			{
				return (Registers.WriteCount > 0 || Notifications.WriteCount > 0);
			}
		}

		/// <summary>
		///   Collection of memory components contained in the AXI Lite interface.
		/// </summary>
		public AddressibleItemCollection Memories { get; private set; }

		/// <summary>
		///   Collection of notification areas contained in the AXI Lite interface. May overlap with the register or memory collections.
		/// </summary>
		/// <remarks>Notifications are used to trigger events when a register is read or written to.</remarks>
		public PartialRegisterInfoCollection Notifications { get; private set; }

		/// <summary>
		///   Collection of register components contained in the AXI Lite interface.
		/// </summary>
		public PartialRegisterInfoCollection Registers { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="SlaveBusLite"/> object.
		/// </summary>
		/// <param name="minAddressWidth">Minimum address width of the AXI Lite interface.</param>
		/// <param name="name">Name of the interface. If null, then the name will be generated based on the interface type.</param>
		/// <param name="singleBus">True if this is the only AXI-Lite bus in the module. False otherwise.</param>
		public SlaveBusLite(int minAddressWidth, string name = null, bool singleBus = true)
			: base(InterfaceType.Slave, minAddressWidth, name, singleBus)
		{
			Memories = new AddressibleItemCollection(false);
			Registers = new PartialRegisterInfoCollection(RegisterWidth);
			Notifications = new PartialRegisterInfoCollection(RegisterWidth);
		}

		/// <summary>
		///   Generates the required logic for the bus and adds the logic to the specified module.
		/// </summary>
		/// <param name="mod"><see cref="ModuleInfo"/> to add the bus to.</param>
		/// <exception cref="ArgumentNullException"><paramref name="mod"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">
		///   Overlaps occurred in the <see cref="Memories"/> and/or <see cref="Registers"/>.
		/// </exception>
		public override void Generate(ModuleInfo mod)
		{
			if (mod == null)
				throw new ArgumentNullException(nameof(mod));

			GenerateEntityComponents(mod.Entity, true, true);
			GenerateDeclaredTypes(mod, Response.OKAY, Response.SLVERR);
			GenerateReadProcessing(mod);
			GenerateWriteProcessing(mod);
		}

		/// <summary>
		///   Generates all the VHDL items that are required for read processing.
		/// </summary>
		/// <param name="module"><see cref="ModuleInfo"/> to add the items to.</param>
		/// <exception cref="InvalidOperationException">
		///   Overlaps occurred in the <see cref="Memories"/> and/or <see cref="Registers"/>.
		/// </exception>
		private void GenerateReadProcessing(ModuleInfo module)
		{
			// Check for overlaps.
			string[] overlaps = AddressibleItemCollection.CheckForOverlaps(Access.Read, Registers, Memories);
			if(overlaps != null)
			{
				// Build up an error message.
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("The following error(s) were found when checking for overlapped registers/memories:");
				foreach (string overlap in overlaps)
					sb.AppendLine(overlap);
				throw new InvalidOperationException(sb.ToString());
			}

			// Add the state machine.
			GenerateReadStateMachine(module);

			// Add the read register process.
			GenerateReadResponseProcess(module);

			// Add the address read ready process.
			GenerateReadReadyProcess(module);
		}

		/// <summary>
		///   Generates the read state machine and associated signals and adds them to the specified module.
		/// </summary>
		/// <param name="module">Module to add the state machine to.</param>
		private void GenerateReadStateMachine(ModuleInfo module)
		{
			int addr = MinAddressWidth - 1;
			ProcessInfo proc = new ProcessInfo("READ_STATE_MACHINE", "Contains the logic to track the state of the read transactions on the AXI interface.");
			proc.SensitivityList.Add(ClockName);
			if (MinAddressWidth != AddressLSB)
				module.Signals.Add(new SignalInfo("read_address", VHDLUtility.GetStdLogicType(0, addr - AddressLSB, true), $"Stores the address received on the {GetName(Signal.ARADDR)} while performing the read operation on a memory.", "(others => '0')"));
			proc.CodeLines.Add($"if(rising_edge({ClockName})) then");
			proc.CodeLines.Add($"	if({ResetName} = '0') then");
			if (MinAddressWidth != AddressLSB)
				proc.CodeLines.Add("		read_address <= (others => '0');");
			proc.CodeLines.Add("		read_state <= sIdle;");

			// Determine all the possible states.
			StringBuilder sb = new StringBuilder();
			sb.Append("(sIdle, sReadRegister");
			if (Memories.Count > 0) 
			{
				string[] designators = Memories.GetDesignators();
				foreach(string designator in designators)
				{
					MemoryBlockInfo block = Memories[designator] as MemoryBlockInfo;
					module.Signals.Add(new SignalInfo(block.ReadNotificationSignalName, "std_logic",
						$"Asserted for a single clock cycle when the {block.Name} should be read.", "'0'"));
					proc.CodeLines.Add($"		{block.ReadNotificationSignalName} <= '0';");
					sb.Append(", ");
					sb.Append(block.StateName);
				}
				module.Signals.Add(new SignalInfo("count", "integer range 0 to C_MEM_LATENCY", "Tracks the number of clock transitions before a read is valid on the memories.", "0"));
				proc.CodeLines.Add("		count <= 0;");
			}
			sb.Append(", sWaitResponse)");

			// Create the state type and signals.
			module.DeclaredTypes.Add(new DeclarationInfo(DeclarationType.Type, "READ_STATE_T", sb.ToString(), "Various states of the Read State Machine"));
			module.Signals.Add(new SignalInfo("read_state", "READ_STATE_T", "Tracks the current state of the read state machine.", "sIdle"));

			// Add the rest of the code.
			proc.CodeLines.Add("	else");
			if (MinAddressWidth != AddressLSB)
				proc.CodeLines.Add("		read_address <= read_address;");
			proc.CodeLines.Add("		read_state <= read_state;");
			if (Memories.Count > 0)
			{
				string[] designators = Memories.GetDesignators();
				foreach (string designator in designators)
				{
					proc.CodeLines.Add($"		{((MemoryBlockInfo)Memories[designator]).ReadNotificationSignalName} <= '0';");
				}
				proc.CodeLines.Add("		count <= count;");
			}
			proc.CodeLines.Add("		case read_state is");
			proc.CodeLines.Add("			when sIdle =>");
			proc.CodeLines.Add($"				if({GetName(Signal.ARVALID)} = '1') then");
			proc.CodeLines.Add($"					if({GetName(Signal.ARADDR)}({AddressLSB - 1} downto 0) /= {VHDLUtility.GetBitString(0, AddressLSB, true)}) then");
			proc.CodeLines.Add("						-- Address does not fall on a register boundary so send a SLVERR response.");
			proc.CodeLines.Add("						read_state <= sWaitResponse;");
			proc.CodeLines.Add($"					elsif({AddressWidthAttributeName} > {MinAddressWidth} and unsigned({GetName(Signal.ARADDR)}({AddressWidthAttributeName}-1 downto {MinAddressWidth})) /= 0) then");
			proc.CodeLines.Add("						-- The address is outside of the valid register space so just return the default value.");
			proc.CodeLines.Add("						read_state <= sWaitResponse;");
			proc.CodeLines.Add("					else");
			proc.CodeLines.Add("						-- Read Address Transaction");
			if (MinAddressWidth != AddressLSB)
				proc.CodeLines.Add($"						read_address <= {GetName(Signal.ARADDR)}({addr} downto {AddressLSB});");
			bool first = true;
			string start;
			ulong[] readableMemories = Memories.GetOffsets(Access.Read);
			if (Memories.Count > 0)
			{
				foreach (ulong offset in readableMemories)
				{
					MemoryBlockInfo block = Memories[Access.Read, offset] as MemoryBlockInfo;
					start = first ? "						if" : "						elsif";
					first = false;
					string startAddress = VHDLUtility.GetBitString(offset, MinAddressWidth, true, AddressLSB);
					string endAddress = VHDLUtility.GetBitString(offset + (ulong)block.Length - 1, MinAddressWidth, true, AddressLSB);
					proc.CodeLines.Add($"{start}({GetName(Signal.ARADDR)}({addr} downto {AddressLSB}) >= {startAddress} and {GetName(Signal.ARADDR)}({addr} downto {AddressLSB}) <= {endAddress}) then");
					proc.CodeLines.Add($"							{block.ReadNotificationSignalName} <= '1';");
					proc.CodeLines.Add("							count <= 0;");
					proc.CodeLines.Add($"							read_state <= {block.StateName};");
				}
				proc.CodeLines.Add("						else");
				proc.CodeLines.Add("							read_state <= sReadRegister;");
				proc.CodeLines.Add("						end if;");
			}
			else
			{
				proc.CodeLines.Add("						read_state <= sReadRegister;");
			}
			proc.CodeLines.Add("					end if;");
			proc.CodeLines.Add("				end if;");
			foreach (ulong offset in readableMemories)
			{
				MemoryBlockInfo block = Memories[Access.Read, offset] as MemoryBlockInfo;
				proc.CodeLines.Add($"			when {block.StateName} => -- {Memories[Access.Read, offset].Name}");
				proc.CodeLines.Add("				if(count = C_MEM_LATENCY) then");
				proc.CodeLines.Add("					read_state <= sWaitResponse;");
				proc.CodeLines.Add("				else");
				proc.CodeLines.Add("					count <= count + 1;");
				proc.CodeLines.Add("				end if;");
			}
			proc.CodeLines.Add("			when sReadRegister =>");
			proc.CodeLines.Add("				-- Wait a single clock cycle.");
			proc.CodeLines.Add("				read_state <= sWaitResponse;");
			proc.CodeLines.Add("			when sWaitResponse =>");
			proc.CodeLines.Add($"				if({GetName(Signal.RREADY)} = '1') then");
			proc.CodeLines.Add("					read_state <= sIdle;");
			proc.CodeLines.Add("				end if;");
			proc.CodeLines.Add("		end case;");
			proc.CodeLines.Add("	end if;");
			proc.CodeLines.Add("end if;");
			module.Processes.Add(proc);
		}

		/// <summary>
		///   Generates the read response process and associated signals and adds them to the specified module.
		/// </summary>
		/// <param name="module"><see cref="ModuleInfo"/> to add the process and signals to.</param>
		private void GenerateReadResponseProcess(ModuleInfo module)
		{
			module.Signals.Add(new SignalInfo("rresp", "std_logic_vector(1 downto 0)", $"Registers the value to be sent on the {GetName(Signal.RRESP)} port.", "(others => '0')", null, true));
			module.ConcurrentStatements.Add($"{GetName(Signal.RRESP)} <= rresp;");
			//module.ConcurrentStatements.Add($"{GetName(Signal.RRESP)} <= OKAY;");
			module.Signals.Add(new SignalInfo("rvalid", "std_logic", $"Registers the value to be sent on the {GetName(Signal.RVALID)} port.", "'0'", null, true));
			module.ConcurrentStatements.Add($"{GetName(Signal.RVALID)} <= rvalid;");
			module.Signals.Add(new SignalInfo("rdata", $"std_logic_vector({DataWidthAttributeName}-1 downto 0)", $"The return value of the read transaction. Sent on the {GetName(Signal.RDATA)} port.", "(others => '0')", null, true));
			module.ConcurrentStatements.Add($"{GetName(Signal.RDATA)} <= rdata;");

			ProcessInfo proc = new ProcessInfo("READ_RESPONSE", "Contains the logic to control the read response signals.");
			proc.SensitivityList.Add("rresp");
			proc.SensitivityList.Add("rvalid");
			proc.SensitivityList.Add("rdata");
			proc.SensitivityList.Add("read_state");
			proc.CodeLines.Add("next_rresp <= rresp;");
			proc.CodeLines.Add("next_rvalid <= rvalid;");
			proc.CodeLines.Add("next_rdata <= rdata;");
			proc.CodeLines.Add("case read_state is");
			proc.CodeLines.Add("	when sIdle =>");
			proc.SensitivityList.Add(GetName(Signal.ARVALID));
			proc.SensitivityList.Add(GetName(Signal.ARADDR));
			proc.CodeLines.Add($"		if({GetName(Signal.ARVALID)} = '1') then");
			proc.CodeLines.Add($"			if({GetName(Signal.ARADDR)}({AddressLSB - 1} downto 0) /= {VHDLUtility.GetBitString(0, AddressLSB, true)}) then");
			proc.CodeLines.Add("				-- The address does not fall on a register boundary so flag it as an error on the bus.");
			proc.CodeLines.Add("				next_rresp <= SLVERR;");
			proc.CodeLines.Add("				next_rvalid <= '1';");
			proc.CodeLines.Add("				next_rdata <= (others => '0');");
			proc.CodeLines.Add($"			elsif({AddressWidthAttributeName} > {MinAddressWidth} and unsigned({GetName(Signal.ARADDR)}({AddressWidthAttributeName}-1 downto {MinAddressWidth})) /= 0) then");
			proc.CodeLines.Add("				-- The address is outside of the valid register space so just return the default value.");
			proc.CodeLines.Add("				next_rresp <= OKAY;");
			proc.CodeLines.Add("				next_rvalid <= '1';");
			proc.CodeLines.Add($"				next_rdata <= {VHDLUtility.GetBitString(DefaultRegisterValue, RegisterWidth, true)};");
			proc.CodeLines.Add("			end if;");
			proc.CodeLines.Add("		end if;");
			if (Memories.Count > 0)
			{
				proc.SensitivityList.Add("count");
				ulong[] readableMemories = Memories.GetOffsets(Access.Read);
				foreach (ulong offset in readableMemories)
				{
					MemoryBlockInfo block = Memories[Access.Read, offset] as MemoryBlockInfo;
					proc.CodeLines.Add($"	when {block.StateName} => -- {Memories[Access.Read, offset].Name}");
					proc.CodeLines.Add("		if(count = C_MEM_LATENCY) then");
					proc.CodeLines.Add("			next_rresp <= OKAY;");
					proc.CodeLines.Add("			next_rvalid <= '1';");
					proc.SensitivityList.Add(block.MemoryOutputRegisterName);
					proc.CodeLines.Add($"			next_rdata <= {block.MemoryOutputRegisterName};");
					proc.CodeLines.Add("		end if;");
				}
			}
			proc.CodeLines.Add("	when sReadRegister =>");
			proc.CodeLines.Add("		next_rresp <= OKAY;");
			proc.CodeLines.Add("		next_rvalid <= '1';");
			bool first = true;
			string start;
			if (Registers.Count > 0)
			{
				if (MinAddressWidth != AddressLSB)
				{
					proc.SensitivityList.Add("read_address");
					ulong[] readableRegisters = Registers.GetOffsets(Access.Read);
					foreach(ulong offset in readableRegisters)
					{
						start = first ? "		if" : "		elsif";
						first = false;
						string value = VHDLUtility.GetBitString(offset, MinAddressWidth, true, AddressLSB);
						proc.CodeLines.Add($"{start}(read_address = {value}) then");
						GenerateRegisterReadCode(proc, Registers.GetItemsAtOffset(offset, Access.Read), true);
					}
					proc.CodeLines.Add("		else");
					proc.CodeLines.Add("			-- Return the default value if reading a non-defined register.");
					proc.CodeLines.Add($"			next_rdata <= {VHDLUtility.GetBitString(DefaultRegisterValue, RegisterWidth)};");
					proc.CodeLines.Add("		end if;");
				}
				else
				{
					// If the minimum address bits matches the bits for a single register then it must just be one register.
					GenerateRegisterReadCode(proc, Registers.GetItemsAtOffset(0, Access.Read));
				}
			}
			else
			{
				proc.CodeLines.Add($"		next_rdata <= {VHDLUtility.GetBitString(DefaultRegisterValue, RegisterWidth)};");
			}
			proc.CodeLines.Add("	when sWaitResponse =>");
			proc.SensitivityList.Add(GetName(Signal.RREADY));
			proc.CodeLines.Add($"		if({GetName(Signal.RREADY)} = '1') then");
			proc.CodeLines.Add("			next_rvalid <= '0';");
			proc.CodeLines.Add("		end if;");
			proc.CodeLines.Add("end case;");
			module.Processes.Add(proc);
		}

		/// <summary>
		///   Generates the individual register read code.
		/// </summary>
		/// <param name="proc"><see cref="ProcessInfo"/> object to add the code lines to.</param>
		/// <param name="regs"><see cref="AddressableItemInfo"/> objects representing the registers to add the code for.</param>
		/// <param name="extraTab">True if an extra tab should be added prior to the code lines, false if it shouldn't be added.</param>
		private void GenerateRegisterReadCode(ProcessInfo proc, AddressableItemInfo[] regs, bool extraTab = false)
		{
			string pre = string.Empty;
			if (extraTab)
				pre = "\t";
			foreach (RegisterValueInfo reg in regs)
			{
				proc.SensitivityList.Add(reg.SignalName);
				if (reg.StartBit == 0 && reg.EndBit == RegisterWidth - 1)
				{
					// Full register.
					if(reg.Type == RegisterType.Unsigned)
						proc.CodeLines.Add($"{pre}		next_rdata <= std_logic_vector({reg.SignalName});");
					else
						proc.CodeLines.Add($"{pre}		next_rdata <= {reg.SignalName};");
				}
				else
				{
					// Partial register.
					proc.CodeLines.Add($"{pre}		next_rdata{VHDLUtility.GetStdLogicArrayIndex(reg.StartBit, reg.EndBit, true, false)} <= {reg.SignalName};");
				}
			}
		}

		/// <summary>
		///   Generates the read ready process. Process to control the read ready port.
		/// </summary>
		/// <param name="module"><see cref="ModuleInfo"/> object to add the process to.</param>
		private void GenerateReadReadyProcess(ModuleInfo module)
		{
			module.Signals.Add(new SignalInfo("arready", "std_logic", $"Asserted when the core is ready for a read transaction. Sent on {GetName(Signal.RRESP)} port.", "'0'", null, true));
			module.ConcurrentStatements.Add($"{GetName(Signal.ARREADY)} <= arready;");

			int addr = MinAddressWidth - 1;
			ProcessInfo proc = new ProcessInfo("READ_READY", "Contains the logic to control the address read ready signal.");
			proc.SensitivityList.Add("arready");
			proc.SensitivityList.Add("read_state");
			proc.SensitivityList.Add(GetName(Signal.ARVALID));
			proc.SensitivityList.Add(GetName(Signal.RREADY));
			proc.CodeLines.Add("next_arready <= arready;");
			proc.CodeLines.Add("if(read_state = sIdle) then");
			proc.CodeLines.Add($"	if({GetName(Signal.ARVALID)} = '1') then");
			proc.CodeLines.Add("		-- Read transaction started so signal we aren't ready until this transaction is complete.");
			proc.CodeLines.Add("		next_arready <= '0';");
			proc.CodeLines.Add("	else");
			proc.CodeLines.Add("		-- This ensures it will be set to a '1' after reset.");
			proc.CodeLines.Add("		next_arready <= '1';");
			proc.CodeLines.Add("	end if;");
			proc.CodeLines.Add("elsif(read_state = sWaitResponse) then");
			proc.CodeLines.Add($"	if({GetName(Signal.RREADY)} = '1') then");
			proc.CodeLines.Add("		-- Finished with the current transaction.");
			proc.CodeLines.Add("		next_arready <= '1';");
			proc.CodeLines.Add("	end if;");
			proc.CodeLines.Add("end if;");
			module.Processes.Add(proc);
		}

		/// <summary>
		///   Generates all the VHDL items that are required for write processing.
		/// </summary>
		/// <param name="module"><see cref="ModuleInfo"/> object to add the items to.</param>
		/// <exception cref="InvalidOperationException">
		///   Overlaps occurred in the <see cref="Memories"/> and/or <see cref="Registers"/>.
		/// </exception>
		private void GenerateWriteProcessing(ModuleInfo module)
		{
			if (module == null)
				throw new ArgumentNullException(nameof(module));

			// Check for overlaps.
			string[] overlaps = AddressibleItemCollection.CheckForOverlaps(Access.Write, Registers, Memories);
			if (overlaps != null)
			{
				// Build up an error message.
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("The following error(s) were found when checking for overlapped registers/memories:");
				foreach (string overlap in overlaps)
					sb.AppendLine(overlap);
				throw new InvalidOperationException(sb.ToString());
			}

			// Add the state machine.
			GenerateWriteStateMachine(module);

			// Add the write register process.
			GenerateWriteRegisterProcess(module);

			// Add the write response process.
			GenerateWriteResponseProcess(module);
			GenerateWriteResponseValidProcess(module);

			// Add the address write ready process.
			GenerateWriteReadyProcess(module);
		}

		/// <summary>
		///   Generates the write state machine process and associated signals.
		/// </summary>
		/// <param name="module"><see cref="ModuleInfo"/> object to add the process to.</param>
		private void GenerateWriteStateMachine(ModuleInfo module)
		{
			int addr = MinAddressWidth - 1;
			ProcessInfo proc = new ProcessInfo("WRITE_STATE_MACHINE", "Contains the logic to track the state of a write transaction on the AXI interface.");
			proc.SensitivityList.Add(ClockName);
			module.DeclaredTypes.Add(new DeclarationInfo(DeclarationType.Type, "WRITE_STATE_T", "(sIdle, sWaitAddress, sWaitData, sWaitResponse)", "Various states of the Write State Machine"));
			module.Signals.Add(new SignalInfo("write_state", "WRITE_STATE_T", "Tracks the current state of the write state machine.", "sIdle"));

			AddressableItemInfo[] writeableMemories = Memories.GetItems(Access.Write);
			proc.CodeLines.Add($"if(rising_edge({ClockName})) then");
			proc.CodeLines.Add($"	if({ResetName} = '0') then");
			if (HasWriteableBlocks)
			{
				if (MinAddressWidth != AddressLSB)
					module.Signals.Add(new SignalInfo("write_address", VHDLUtility.GetStdLogicType(0, addr - AddressLSB, true), $"Stores the address received on the {GetName(Signal.AWADDR)} while performing the write operation.", "(others => '0')"));
				module.Signals.Add(new SignalInfo("write_strb", $"std_logic_vector(({DataWidthAttributeName}/8)-1 downto 0)", "Determines which bytes in write_data should be written during the write operation.", "(others => '0')"));
				module.Signals.Add(new SignalInfo("write_data", $"std_logic_vector({DataWidthAttributeName}-1 downto 0)", $"Contains the data from {GetName(Signal.WDATA)} to be written to the components during the write operation.", "(others => '0')"));
				if (MinAddressWidth != AddressLSB)
					proc.CodeLines.Add("		write_address <= (others => '0');");
				proc.CodeLines.Add("		write_strb <= (others => '0');");
				proc.CodeLines.Add("		write_data <= (others => '0');");
			}
			proc.CodeLines.Add("		write_state <= sIdle;");
			if (HasWriteableRegisters)
			{
				module.Signals.Add(new SignalInfo("write_reg", "std_logic", "Asserted for a single clock cycle when one of the registers should be written to.", "'0'"));
				proc.CodeLines.Add("		write_reg <= '0';");
			}
			foreach (MemoryBlockInfo mem in writeableMemories)
			{
				module.Signals.Add(new SignalInfo(mem.WriteNotificationSignalName, "std_logic", $"Asserted for a single clock cycle when the {mem.Name} memory component should be written to.", "'0'"));
				proc.CodeLines.Add($"		{mem.WriteNotificationSignalName} <= '0';");
			}
			proc.CodeLines.Add("	else");
			if (HasWriteableBlocks)
			{
				if (MinAddressWidth != AddressLSB)
					proc.CodeLines.Add("		write_address <= write_address;");
				proc.CodeLines.Add("		write_strb <= write_strb;");
				proc.CodeLines.Add("		write_data <= write_data;");
			}
			proc.CodeLines.Add("		write_state <= write_state;");
			if (Registers.WriteCount > 0 || Notifications.WriteCount > 0)
				proc.CodeLines.Add("		write_reg <= '0';");
			foreach (MemoryBlockInfo mem in writeableMemories)
				proc.CodeLines.Add($"		{mem.WriteNotificationSignalName} <= '0';");

			proc.CodeLines.Add("		case write_state is");
			proc.CodeLines.Add("			when sIdle =>");
			proc.CodeLines.Add($"				if ({GetName(Signal.AWVALID)} = '1') then");
			proc.CodeLines.Add("					-- Write Address Transaction.");
			if (HasWriteableBlocks && (MinAddressWidth != AddressLSB))
				proc.CodeLines.Add($"					write_address <= {GetName(Signal.AWADDR)}({addr} downto {AddressLSB});");
			proc.CodeLines.Add($"					if ({GetName(Signal.WVALID)} = '1') then");
			proc.CodeLines.Add("						-- Write Data Transaction.");
			if (HasWriteableBlocks)
			{
				proc.CodeLines.Add($"						if({GetName(Signal.AWADDR)}({AddressLSB - 1} downto 0) /= {VHDLUtility.GetBitString(0, AddressLSB, true)}) then");
				proc.CodeLines.Add("							-- Address does not fall on a register boundary so flag an error on the bus.");
				proc.CodeLines.Add("							write_state <= sWaitResponse;");
				proc.CodeLines.Add($"						elsif({AddressWidthAttributeName} > {MinAddressWidth} and unsigned({GetName(Signal.AWADDR)}({AddressWidthAttributeName}-1 downto {MinAddressWidth})) /= 0) then");
				proc.CodeLines.Add("							-- The address is outside of the valid register space so flag an error on the bus.");
				proc.CodeLines.Add("							write_state <= sWaitResponse;");
				proc.CodeLines.Add("						else");
				proc.CodeLines.Add($"							write_data <= {GetName(Signal.WDATA)};");
				proc.CodeLines.Add($"							write_strb <= {GetName(Signal.WSTRB)};");
				GenerateWriteStateMachineComponentCode(proc, true);
				proc.CodeLines.Add("							write_state <= sWaitResponse;");
				proc.CodeLines.Add("						end if;");
			}
			else
			{
				proc.CodeLines.Add("						write_state <= sWaitResponse;");
			}
			proc.CodeLines.Add("					else");
			proc.CodeLines.Add("						write_state <= sWaitData;");
			proc.CodeLines.Add("					end if;");
			proc.CodeLines.Add("				else");
			proc.CodeLines.Add($"					if ({GetName(Signal.WVALID)} = '1') then");
			proc.CodeLines.Add("						-- Write Data Transaction.");
			if (HasWriteableBlocks)
			{
				proc.CodeLines.Add($"						write_data <= {GetName(Signal.WDATA)};");
				proc.CodeLines.Add($"						write_strb <= {GetName(Signal.WSTRB)};");
			}
			proc.CodeLines.Add("						write_state <= sWaitAddress;");
			proc.CodeLines.Add("					end if;");
			proc.CodeLines.Add("				end if;");
			proc.CodeLines.Add("			when sWaitAddress =>");
			proc.CodeLines.Add($"				if ({GetName(Signal.AWVALID)} = '1') then");
			if (HasWriteableBlocks)
			{
				proc.CodeLines.Add($"					if({GetName(Signal.AWADDR)}({AddressLSB - 1} downto 0) /= {VHDLUtility.GetBitString(0, AddressLSB, true)}) then");
				proc.CodeLines.Add("						-- Address does not fall on a register boundary so flag an error on the bus.");
				proc.CodeLines.Add("						write_state <= sWaitResponse;");
				proc.CodeLines.Add($"					elsif({AddressWidthAttributeName} > {MinAddressWidth} and unsigned({GetName(Signal.AWADDR)}({AddressWidthAttributeName}-1 downto {MinAddressWidth})) /= 0) then");
				proc.CodeLines.Add("						-- The address is outside of the valid register space so flag an error on the bus.");
				proc.CodeLines.Add("						write_state <= sWaitResponse;");
				proc.CodeLines.Add("					else");
				if (MinAddressWidth != AddressLSB)
					proc.CodeLines.Add($"						write_address <= {GetName(Signal.AWADDR)}({addr} downto {AddressLSB});");
				GenerateWriteStateMachineComponentCode(proc);
				proc.CodeLines.Add("						write_state <= sWaitResponse;");
				proc.CodeLines.Add("					end if;");
			}
			else
			{
				proc.CodeLines.Add("					write_state <= sWaitResponse;");
			}
			proc.CodeLines.Add("				end if;");
			proc.CodeLines.Add("			when sWaitData =>");
			proc.CodeLines.Add($"				if ({GetName(Signal.WVALID)} = '1') then");
			if (HasWriteableBlocks)
			{
				proc.CodeLines.Add("					if (bresp = OKAY) then");
				proc.CodeLines.Add($"						write_data <= {GetName(Signal.WDATA)};");
				proc.CodeLines.Add($"						write_strb <= {GetName(Signal.WSTRB)};");
				GenerateWriteStateMachineComponentCode(proc, false, true);
				proc.CodeLines.Add("					end if;");
			}
			proc.CodeLines.Add("					write_state <= sWaitResponse;");
			proc.CodeLines.Add("				end if;");
			proc.CodeLines.Add("			when sWaitResponse =>");
			proc.CodeLines.Add($"				if ({GetName(Signal.BREADY)} = '1') then");
			proc.CodeLines.Add("					write_state <= sIdle;");
			proc.CodeLines.Add("				end if;");
			proc.CodeLines.Add("		end case;");
			proc.CodeLines.Add("	end if;");
			proc.CodeLines.Add("end if;");
			module.Processes.Add(proc);
		}

		/// <summary>
		///   Generates the write state machine individual component code.
		/// </summary>
		/// <param name="proc"><see cref="ProcessInfo"/> object representing the process to add the individual code to.</param>
		/// <param name="extraTab">True if an extra tab should be added at the first of each line. False otherwise.</param>
		/// <param name="useWriteAddress">True if 'write_address' should be used for the address instead of pulling it from AWADDR directly.</param>
		private void GenerateWriteStateMachineComponentCode(ProcessInfo proc, bool extraTab = false, bool useWriteAddress = false)
		{
			string pre = extraTab ? "\t" : string.Empty;
			if (MinAddressWidth != AddressLSB)
			{
				int addr = MinAddressWidth - 1;
				bool first = true;
				string source = useWriteAddress ? "write_address" : $"{GetName(Signal.AWADDR)}({addr} downto {AddressLSB})";
				AddressableItemInfo[] mems = Memories.GetItems(Access.Write);
				foreach (MemoryBlockInfo mem in mems)
				{
					string start = first ? $"{pre}						if" : $"{pre}						elsif";
					first = false;
					string startAddress = VHDLUtility.GetBitString(mem.Offset, addr + 1, true, AddressLSB);
					string endAddress = VHDLUtility.GetBitString(mem.Offset + (ulong)mem.Length - 1, addr + 1, true, AddressLSB);
					proc.CodeLines.Add($"{start}({source} >= {startAddress} and {source} <= {endAddress}) then");
					proc.CodeLines.Add($"{pre}							{mem.WriteNotificationSignalName} <= '1';");
				}
				Tuple<ulong, int>[] blocks = AddressibleItemCollection.GetSimplifiedBlocks(Access.Write, Registers, Notifications);
				foreach (Tuple<ulong, int> block in blocks)
				{
					string start = first ? $"{pre}						if" : $"{pre}						elsif";
					first = false;
					if (block.Item2 == RegisterByteWidth)
					{
						proc.CodeLines.Add($"{start}({source} = {VHDLUtility.GetBitString(block.Item1, addr + 1, true, AddressLSB)}) then");
					}
					else
					{
						string startAddress = VHDLUtility.GetBitString(block.Item1, addr + 1, true, AddressLSB);
						string endAddress = VHDLUtility.GetBitString(block.Item1 + (ulong)block.Item2 - 1, addr + 1, true, AddressLSB);
						proc.CodeLines.Add($"{start}({source} >= {startAddress} and {source} <= {endAddress}) then");
					}
					proc.CodeLines.Add(pre + "							write_reg <= '1';");
				}
				proc.CodeLines.Add($"{pre}						end if;");
			}
			else
			{
				// Single register or memory so write to it directly.
				if (Memories.WriteCount == 1)
				{
					// Memory.
					proc.CodeLines.Add($"{pre}						{((MemoryBlockInfo)Memories[Access.Write, 0]).WriteNotificationSignalName} <= '1';");
				}
				else
				{
					// Register.
					proc.CodeLines.Add(pre + "						write_reg <= '1';");
				}
			}
		}

		/// <summary>
		///   Generate the write ready process. Process that contains the logic to drive the write ready port.
		/// </summary>
		/// <param name="module"><see cref="ModuleInfo"/> object to add the process to.</param>
		private void GenerateWriteReadyProcess(ModuleInfo module)
		{
			module.Signals.Add(new SignalInfo("awready", "std_logic", $"Asserted when the core is ready for a write address transaction. Sent on {GetName(Signal.AWREADY)} port.", "'0'", null, true));
			module.ConcurrentStatements.Add($"{GetName(Signal.AWREADY)} <= awready;");
			module.Signals.Add(new SignalInfo("wready", "std_logic", $"Asserted when the core is ready for a write data transaction. Sent on {GetName(Signal.WREADY)} port.", "'0'", null, true));
			module.ConcurrentStatements.Add($"{GetName(Signal.WREADY)} <= wready;");

			ProcessInfo proc = new ProcessInfo("WRITE_READY", "Contains the logic to control the address write ready signals.");
			proc.SensitivityList.Add("awready");
			proc.SensitivityList.Add("wready");
			proc.SensitivityList.Add("write_state");
			proc.SensitivityList.Add(GetName(Signal.AWVALID));
			proc.SensitivityList.Add(GetName(Signal.WVALID));
			proc.SensitivityList.Add(GetName(Signal.BREADY));
			proc.CodeLines.Add("next_awready <= awready;");
			proc.CodeLines.Add("next_wready <= wready;");
			proc.CodeLines.Add("case write_state is");
			proc.CodeLines.Add("	when sIdle =>");
			proc.CodeLines.Add("		-- Assert the ready signals that are not currently being read on this clock cycle.");
			proc.CodeLines.Add($"		if ({GetName(Signal.AWVALID)} = '1') then");
			proc.CodeLines.Add($"			if ({GetName(Signal.WVALID)} = '1') then");
			proc.CodeLines.Add("				next_awready <= '0';");
			proc.CodeLines.Add("				next_wready <= '0';");
			proc.CodeLines.Add("			else");
			proc.CodeLines.Add("				next_awready <= '0';");
			proc.CodeLines.Add("				next_wready <= '1';");
			proc.CodeLines.Add("			end if;");
			proc.CodeLines.Add("		else");
			proc.CodeLines.Add($"			if ({GetName(Signal.WVALID)} = '1') then");
			proc.CodeLines.Add("				next_awready <= '1';");
			proc.CodeLines.Add("				next_wready <= '0';");
			proc.CodeLines.Add("			else");
			proc.CodeLines.Add("				next_awready <= '1';");
			proc.CodeLines.Add("				next_wready <= '1';");
			proc.CodeLines.Add("			end if;");
			proc.CodeLines.Add("		end if;");
			proc.CodeLines.Add("	when sWaitAddress =>");
			proc.CodeLines.Add($"		if ({GetName(Signal.AWVALID)} = '1') then");
			proc.CodeLines.Add("			next_awready <= '0';");
			proc.CodeLines.Add("		end if;");
			proc.CodeLines.Add("	when sWaitData =>");
			proc.CodeLines.Add($"		if ({GetName(Signal.WVALID)} = '1') then");
			proc.CodeLines.Add("			next_wready <= '0';");
			proc.CodeLines.Add("		end if;");
			proc.CodeLines.Add("	when sWaitResponse =>");
			proc.CodeLines.Add($"		if ({GetName(Signal.BREADY)} = '1') then");
			proc.CodeLines.Add("			next_awready <= '1';");
			proc.CodeLines.Add("			next_wready <= '1';");
			proc.CodeLines.Add("		end if;");
			proc.CodeLines.Add("end case;");
			module.Processes.Add(proc);
		}

		/// <summary>
		///   Generates the write register process. Contains the logic to actually write to the individual signals.
		/// </summary>
		/// <param name="info"><see cref="ModuleInfo"/> object to add the process to.</param>
		private void GenerateWriteRegisterProcess(ModuleInfo info)
		{
			if (Registers.WriteCount == 0)
				return;

			ProcessInfo proc = new ProcessInfo("WRITE_REGISTER", "Contains the logic to handle writes to registers.");
			ulong[] writeableOffsets = AddressibleItemCollection.GetUniqueOffsets(Access.Write, Registers, Notifications);
			foreach (ulong offset in writeableOffsets)
			{
				AddressableItemInfo[] vals = Registers.GetItemsAtOffset(offset, Access.Write);
				foreach (AddressableItemInfo val in vals)
				{
					RegisterValueInfo reg = val as RegisterValueInfo;
					info.Signals.Add(new SignalInfo(reg.SignalName, reg.SignalType, reg.Description, reg.DefaultValue, null, true));
					proc.SensitivityList.Add(reg.SignalName);
					proc.CodeLines.Add($"next_{reg.SignalName} <= {reg.SignalName};");
				}

				vals = Notifications.GetItemsAtOffset(offset, Access.Write);
				foreach (AddressableItemInfo val in vals)
				{
					NotificationInfo reg = val as NotificationInfo;
					info.Signals.Add(new SignalInfo(reg.WriteSignalName, "std_logic", reg.Description, "'0'", null, true));
					proc.CodeLines.Add($"next_{reg.WriteSignalName} <= '0';");
				}
			}

			proc.SensitivityList.Add("write_reg");
			proc.CodeLines.Add("if(write_reg = '1') then");

			bool first = true;
			if (MinAddressWidth != AddressLSB)
				proc.SensitivityList.Add("write_address");
			proc.SensitivityList.Add("write_strb");
			proc.SensitivityList.Add("write_data");
			if (MinAddressWidth != AddressLSB)
			{
				foreach (ulong offset in writeableOffsets)
				{
					string start = first ? "	if" : "	elsif";
					first = false;
					proc.CodeLines.Add($"{start}(write_address = {VHDLUtility.GetBitString(offset, MinAddressWidth, true, AddressLSB)}) then");
					GenerateRegisterWriteCode(info, proc, offset, true);
				}
				proc.CodeLines.Add("	end if;");
			}
			else
			{
				GenerateRegisterWriteCode(info, proc, 0);
			}
			proc.CodeLines.Add("end if;");

			info.Processes.Add(proc);
		}

		/// <summary>
		///   Generate register write code. Creates the code for the specific offset in the registers.
		/// </summary>
		/// <param name="module"><see cref="ModuleInfo"/> object the proces is contained in.</param>
		/// <param name="proc"><see cref="ProcessInfo"/> object the code lines will be added to.</param>
		/// <param name="offset">Offset to add the code for.</param>
		/// <param name="extraTab">True if an extra tab should be added at the front of each line, false otherwise.</param>
		private void GenerateRegisterWriteCode(ModuleInfo module, ProcessInfo proc, ulong offset, bool extraTab = false)
		{
			string pre = extraTab ? "\t" : string.Empty;
			for(int i = 0; i < RegisterByteWidth; i++)
			{
				List<Tuple<PartialRegisterInfo, int, int>> assignments = new List<Tuple<PartialRegisterInfo, int, int>>();
				assignments.AddRange(Registers.GetWriteRegistersInByteLane(i, offset));
				assignments.AddRange(Notifications.GetWriteRegistersInByteLane(i, offset));
				if(assignments.Count > 0)
				{
					string regIndexer;
					string valueIndexer;
					proc.CodeLines.Add($"{pre}	if(write_strb({i}) = '1') then");
					foreach(Tuple<PartialRegisterInfo, int,int> tup in assignments)
					{
						if (tup.Item1 is RegisterValueInfo)
						{
							// Register Value.
							RegisterValueInfo reg = tup.Item1 as RegisterValueInfo;
							switch (reg.Type)
							{
								case RegisterType.StandardLogic:
									regIndexer = VHDLUtility.GetStdLogicArrayIndex(tup.Item2, tup.Item3, true, false);
									if (tup.Item1.BitWidth == 1)
										valueIndexer = string.Empty;
									else
										valueIndexer = VHDLUtility.GetStdLogicArrayIndex(tup.Item2 - reg.StartBit, tup.Item3 - reg.StartBit, true, false);
									proc.CodeLines.Add($"{pre}		next_{reg.SignalName}{valueIndexer} <= write_data{regIndexer};");
									break;
								case RegisterType.Unsigned:
									regIndexer = VHDLUtility.GetStdLogicArrayIndex(tup.Item2, tup.Item3, true, false);
									valueIndexer = VHDLUtility.GetStdLogicArrayIndex(tup.Item2 - reg.StartBit, tup.Item3 - reg.StartBit, true, false);
									proc.CodeLines.Add($"{pre}		next_{reg.SignalName}{valueIndexer} <= unsigned(write_data{regIndexer});");
									break;
								default:
									throw new NotImplementedException();
							}
						}
						else if (tup.Item1 is NotificationInfo)
						{
							// Notification.
							NotificationInfo reg = tup.Item1 as NotificationInfo;
							proc.CodeLines.Add($"{pre}		next_{reg.WriteSignalName} <= '1';");
						}
						else
						{
							throw new NotImplementedException();
						}
					}
					proc.CodeLines.Add($"{pre}	end if;");
				}
			}
		}

		/// <summary>
		///   Generates the write response process. Sends the response on the AXI bus for the write transaction.
		/// </summary>
		/// <param name="module"><see cref="ModuleInfo"/> object to add the process to.</param>
		private void GenerateWriteResponseProcess(ModuleInfo module)
		{
			if (Memories.WriteCount == 0 && Registers.WriteCount == 0)
			{
				// Nothing is writeable so always return a SLVERR response.
				module.ConcurrentStatements.Add($"{GetName(Signal.BRESP)} <= SLVERR;");
				return;
			}

			module.Signals.Add(new SignalInfo("bresp", "std_logic_vector(1 downto 0)", $"Registers the value to be sent on the {GetName(Signal.BRESP)} port.", "(others => '0')", null, true));
			module.ConcurrentStatements.Add($"{GetName(Signal.BRESP)} <= bresp;");

			int addr = MinAddressWidth - 1;
			ProcessInfo proc = new ProcessInfo("WRITE_RESPONSE", "Contains the logic to control the write response.");
			proc.SensitivityList.Add("bresp");
			proc.SensitivityList.Add("write_state");
			proc.SensitivityList.Add(GetName(Signal.AWVALID));
			proc.SensitivityList.Add(GetName(Signal.AWADDR));
			proc.CodeLines.Add("next_bresp <= bresp;");
			proc.CodeLines.Add("if(write_state = sIdle or write_state = sWaitAddress) then");
			proc.CodeLines.Add($"	if({GetName(Signal.AWVALID)} = '1') then");
			proc.CodeLines.Add($"		if({GetName(Signal.AWADDR)}({AddressLSB - 1} downto 0) /= {VHDLUtility.GetBitString(0, AddressLSB, true)}) then");
			proc.CodeLines.Add("			-- Address does not fall on a register boundary so flag an error on the bus.");
			proc.CodeLines.Add("			next_bresp <= SLVERR;");
			proc.CodeLines.Add($"		elsif({AddressWidthAttributeName} > {MinAddressWidth} and unsigned({GetName(Signal.AWADDR)}({AddressWidthAttributeName}-1 downto {MinAddressWidth})) /= 0) then");
			proc.CodeLines.Add("			-- The address is outside of the valid register space so flag an error on the bus.");
			proc.CodeLines.Add("			next_rresp <= SLVERR;");
			proc.CodeLines.Add("		else");

			Tuple<ulong, int>[] blocks = AddressibleItemCollection.GetSimplifiedBlocks(Access.Write, Notifications, Memories, Registers);
			if (MinAddressWidth != AddressLSB)
			{
				bool first = true;
				foreach (Tuple<ulong, int> block in blocks)
				{
					string start = first ? "			if" : "			elsif";
					first = false;
					if (block.Item2 == RegisterByteWidth)
					{
						// Single register.
						proc.CodeLines.Add($"{start}({GetName(Signal.AWADDR)}({addr} downto {AddressLSB}) = {VHDLUtility.GetBitString(block.Item1, addr + 1, true, AddressLSB)}) then");
					}
					else
					{
						// Multiple registers.
						string startAddress = VHDLUtility.GetBitString(block.Item1, addr + 1, true, AddressLSB);
						string endAddress = VHDLUtility.GetBitString(block.Item1 + (ulong)block.Item2 - 1, addr + 1, true, AddressLSB);
						proc.CodeLines.Add($"{start}({GetName(Signal.AWADDR)}({addr} downto {AddressLSB}) >= {startAddress} and {GetName(Signal.AWADDR)}({addr} downto {AddressLSB}) <= {endAddress}) then");
					}
					proc.CodeLines.Add($"				next_bresp <= OKAY;");
				}
				proc.CodeLines.Add("			else");
				proc.CodeLines.Add("				-- Nothing writeable at this location so flag an error on the bus.");
				proc.CodeLines.Add("				next_bresp <= SLVERR;");
				proc.CodeLines.Add("			end if;");
			}
			else
			{
				// Only a single register in the memory space so set response directly without any checks.
				proc.CodeLines.Add("			next_bresp <= OKAY;");
			}
			proc.CodeLines.Add("		end if;");
			proc.CodeLines.Add("	end if;");
			proc.CodeLines.Add("end if;");
			module.Processes.Add(proc);
		}

		/// <summary>
		///   Generate write response valid process. Determines when the BVALID signal should be asserted.
		/// </summary>
		/// <param name="module"><see cref="ModuleInfo"/> object to add the process to.</param>
		private void GenerateWriteResponseValidProcess(ModuleInfo module)
		{
			module.Signals.Add(new SignalInfo("bvalid", "std_logic", $"Registers the value to be sent on the {GetName(Signal.BVALID)} port.", "'0'", null, true));
			module.ConcurrentStatements.Add($"{GetName(Signal.BVALID)} <= bvalid;");

			int addr = MinAddressWidth - 1;
			ProcessInfo proc = new ProcessInfo("WRITE_RESPONSE_VALID", "Contains the logic to control the write response valid signal.");
			proc.SensitivityList.Add("bvalid");
			proc.SensitivityList.Add("write_state");
			proc.SensitivityList.Add(GetName(Signal.WVALID));
			proc.SensitivityList.Add(GetName(Signal.AWVALID));
			proc.SensitivityList.Add(GetName(Signal.BREADY));
			proc.CodeLines.Add("next_bvalid <= bvalid;");
			proc.CodeLines.Add("case write_state is");
			proc.CodeLines.Add("	when sIdle =>");
			proc.CodeLines.Add($"		if({GetName(Signal.AWVALID)} = '1' and {GetName(Signal.WVALID)} = '1') then");
			proc.CodeLines.Add("			next_bvalid <= '1';");
			proc.CodeLines.Add("		end if;");
			proc.CodeLines.Add("	when sWaitAddress =>");
			proc.CodeLines.Add($"		if({GetName(Signal.AWVALID)} = '1') then");
			proc.CodeLines.Add("			next_bvalid <= '1';");
			proc.CodeLines.Add("		end if;");
			proc.CodeLines.Add("	when sWaitData =>");
			proc.CodeLines.Add($"		if({GetName(Signal.WVALID)} = '1') then");
			proc.CodeLines.Add("			next_bvalid <= '1';");
			proc.CodeLines.Add("		end if;");
			proc.CodeLines.Add("	when sWaitResponse =>");
			proc.CodeLines.Add($"		if({GetName(Signal.BREADY)} = '1') then");
			proc.CodeLines.Add("			next_bvalid <= '0';");
			proc.CodeLines.Add("		end if;");
			proc.CodeLines.Add("end case;");
			module.Processes.Add(proc);
		}

		#endregion
	}
}
