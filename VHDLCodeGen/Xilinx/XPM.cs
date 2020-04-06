//********************************************************************************************************************************
// Filename:    XPM.cs
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

namespace VHDLCodeGen.Xilinx
{
	/// <summary>
	///   Adds support for adding Xilinx's XPM macros to an auto-generated design.
	/// </summary>
	public static class XPM
	{
		#region Fields

		/// <summary>
		///   Library to be added to use the XPM macros.
		/// </summary>
		public static string Library = "XPM.VCOMPONENTS.all";

		#endregion

		#region Properties

		/// <summary>
		///   Contains the Memory SPROM component information.
		/// </summary>
		public static ComponentInfo MemorySPROM { get; private set; }

		/// <summary>
		///   Contains the Memory SPRAM component information.
		/// </summary>
		public static ComponentInfo MemorySPRAM { get; private set; }

		/// <summary>
		///   Contains the Clock Domain Crossing Pulse component information.
		/// </summary>
		public static ComponentInfo CDCPulse { get; private set; }

		/// <summary>
		///   Contains the Clock Domain Crossing single bit component information.
		/// </summary>
		public static ComponentInfo CDCSingle { get; private set; }

		/// <summary>
		///   Contains the Clock Domain Crossing Handshake crossing component information.
		/// </summary>
		public static ComponentInfo CDCHandshake { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		///   Static constructor for the class.
		/// </summary>
		/// <remarks>Creates all the components up front.</remarks>
		static XPM()
		{
			MemorySPROM = GetMemory(false);
			MemorySPRAM = GetMemory(true);
			CDCPulse = GetCDCPulse();
			CDCSingle = GetCDCSingle();
			CDCHandshake = GetCDCHandshake();
		}

		/// <summary>
		///   Generates the CDC Hanshake macro component.
		/// </summary>
		/// <returns><see cref="ComponentInfo"/> object containing the macro information.</returns>
		private static ComponentInfo GetCDCHandshake()
		{
			ComponentInfo comp = new ComponentInfo("XPM_CDC_HANDSHAKE",
				"Bus Synchronizer with Full Handshake", null, true);
			comp.Generics.Add(new SimplifiedGenericInfo("DEST_EXT_HSK", "DECIMAL", "1"));
			comp.Generics.Add(new SimplifiedGenericInfo("DEST_SYNC_FF", "DECIMAL", "4"));
			comp.Generics.Add(new SimplifiedGenericInfo("INIT_SYNC_FF", "DECIMAL", "0"));
			comp.Generics.Add(new SimplifiedGenericInfo("SIM_ASSERT_CHK", "DECIMAL", "0"));
			comp.Generics.Add(new SimplifiedGenericInfo("SRC_SYNC_FF", "DECIMAL", "4"));
			comp.Generics.Add(new SimplifiedGenericInfo("WIDTH", "DECIMAL", "1"));
			comp.Ports.Add(new SimplifiedPortInfo("DEST_ACK", PortDirection.In, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("DEST_CLK", PortDirection.In, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("DEST_OUT", PortDirection.Out, "std_logic_vector(WIDTH-1 downto 0)"));
			comp.Ports.Add(new SimplifiedPortInfo("DEST_REQ", PortDirection.Out, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("SRC_CLK", PortDirection.In, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("SRC_IN", PortDirection.In, "std_logic_vector(WIDTH-1 downto 0)"));
			comp.Ports.Add(new SimplifiedPortInfo("SRC_RCV", PortDirection.Out, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("SRC_SEND", PortDirection.In, "std_logic"));
			return comp;
		}

		/// <summary>
		///   Generates the CDC Pulse macro component.
		/// </summary>
		/// <returns><see cref="ComponentInfo"/> object containing the macro information.</returns>
		private static ComponentInfo GetCDCPulse()
		{
			ComponentInfo comp = new ComponentInfo("XPM_CDC_PULSE",
					"Synchronizes a pulse in the source clock domain to the destination clock domain.", null, true);
			comp.Generics.Add(new SimplifiedGenericInfo("DEST_SYNC_FF", "DECIMAL", "4"));
			comp.Generics.Add(new SimplifiedGenericInfo("INIT_SYNC_FF", "DECIMAL", "0"));
			comp.Generics.Add(new SimplifiedGenericInfo("REG_OUTPUT", "DECIMAL", "0"));
			comp.Generics.Add(new SimplifiedGenericInfo("RST_USED", "DECIMAL", "1"));
			comp.Generics.Add(new SimplifiedGenericInfo("SIM_ASSERT_CHK", "DECIMAL", "0"));
			comp.Ports.Add(new SimplifiedPortInfo("DEST_CLK", PortDirection.In, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("DEST_PULSE", PortDirection.Out, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("DEST_RST", PortDirection.In, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("SRC_CLK", PortDirection.In, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("SRC_PULSE", PortDirection.In, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("SRC_RST", PortDirection.In, "std_logic"));
			return comp;
		}

		/// <summary>
		///   Generates the CDC Single macro component.
		/// </summary>
		/// <returns><see cref="ComponentInfo"/> object containing the macro information.</returns>
		private static ComponentInfo GetCDCSingle()
		{
			ComponentInfo comp = new ComponentInfo("XPM_CDC_SINGLE",
					"Synchronizes a one bit signal from the source clock domain to the destination clock domain.", null, true);
			comp.Generics.Add(new SimplifiedGenericInfo("DEST_SYNC_FF", "DECIMAL", "4"));
			comp.Generics.Add(new SimplifiedGenericInfo("INIT_SYNC_FF", "DECIMAL", "0"));
			comp.Generics.Add(new SimplifiedGenericInfo("SIM_ASSERT_CHK", "DECIMAL", "0"));
			comp.Generics.Add(new SimplifiedGenericInfo("SRC_INPUT_REG", "DECIMAL", "1"));
			comp.Ports.Add(new SimplifiedPortInfo("DEST_CLK", PortDirection.In, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("DEST_OUT", PortDirection.Out, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("SRC_CLK", PortDirection.In, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("SRC_IN", PortDirection.In, "std_logic"));
			return comp;
		}

		/// <summary>
		///   Generates the memory macro component.
		/// </summary>
		/// <param name="ram">True if the macro is for the RAM, false if it is for ROM.</param>
		/// <returns><see cref="ComponentInfo"/> object containing the macro information.</returns>
		private static ComponentInfo GetMemory(bool ram)
		{
			string name = "XPM_MEMORY_SDPRAM";
			if (!ram)
				name = "XPM_MEMORY_SPROM";
			ComponentInfo comp = new ComponentInfo(name, "Xilinx Memory Macro", null, true);
			comp.Generics.Add(new SimplifiedGenericInfo("ADDR_WIDTH_A", "DECIMAL", "6"));
			if (ram)
				comp.Generics.Add(new SimplifiedGenericInfo("ADDR_WIDTH_B", "DECIMAL", "6"));
			comp.Generics.Add(new SimplifiedGenericInfo("AUTO_SLEEP_TIME", "DECIMAL", "0"));
			if (ram)
			{
				comp.Generics.Add(new SimplifiedGenericInfo("BYTE_WRITE_WIDTH_A", "DECIMAL", "32"));
				comp.Generics.Add(new SimplifiedGenericInfo("CLOCKING_MODE", "STRING", "\"common_clock\""));
			}
			comp.Generics.Add(new SimplifiedGenericInfo("ECC_MODE", "STRING", "\"no_ecc\""));
			comp.Generics.Add(new SimplifiedGenericInfo("MEMORY_INIT_FILE", "STRING", "\"none\""));
			comp.Generics.Add(new SimplifiedGenericInfo("MEMORY_INIT_PARAM", "STRING", "\"0\""));
			comp.Generics.Add(new SimplifiedGenericInfo("MEMORY_OPTIMIZATION", "STRING", "\"true\""));
			comp.Generics.Add(new SimplifiedGenericInfo("MEMORY_PRIMITIVE", "STRING", "\"auto\""));
			comp.Generics.Add(new SimplifiedGenericInfo("MEMORY_SIZE", "DECIMAL", "2048"));
			comp.Generics.Add(new SimplifiedGenericInfo("MESSAGE_CONTROL", "DECIMAL", "0"));
			if (ram)
			{
				comp.Generics.Add(new SimplifiedGenericInfo("READ_DATA_WIDTH_B", "DECIMAL", "32"));
				comp.Generics.Add(new SimplifiedGenericInfo("READ_LATENCY_B", "DECIMAL", "2"));
				comp.Generics.Add(new SimplifiedGenericInfo("READ_RESET_VALUE_B", "DECIMAL", "2"));
				comp.Generics.Add(new SimplifiedGenericInfo("USE_EMBEDDED_CONSTRAINT", "DECIMAL", "0"));
			}
			else
			{
				comp.Generics.Add(new SimplifiedGenericInfo("READ_DATA_WIDTH_A", "DECIMAL", "32"));
				comp.Generics.Add(new SimplifiedGenericInfo("READ_LATENCY_A", "DECIMAL", "2"));
				comp.Generics.Add(new SimplifiedGenericInfo("READ_RESET_VALUE_A", "DECIMAL", "2"));
				comp.Generics.Add(new SimplifiedGenericInfo("RST_MODE_A", "STRING", "\"SYNC\""));
			}
			comp.Generics.Add(new SimplifiedGenericInfo("USE_MEM_INIT", "DECIMAL", "1"));
			comp.Generics.Add(new SimplifiedGenericInfo("WAKEUP_TIME", "STRING", "\"disable_sleep\""));
			if (ram)
			{
				comp.Generics.Add(new SimplifiedGenericInfo("WRITE_DATA_WIDTH_A", "DECIMAL", "32"));
				comp.Generics.Add(new SimplifiedGenericInfo("WRITE_MODE_B", "STRING", "\"no_change\""));
			}
			if (ram)
			{
				comp.Ports.Add(new SimplifiedPortInfo("dbiterrb", PortDirection.Out, "std_logic"));
				comp.Ports.Add(new SimplifiedPortInfo("doutb", PortDirection.Out, "std_logic_vector(READ_DATA_WIDTH_B-1 downto 0)"));
				comp.Ports.Add(new SimplifiedPortInfo("sbiterrb", PortDirection.Out, "std_logic"));
			}
			else
			{
				comp.Ports.Add(new SimplifiedPortInfo("dbiterra", PortDirection.Out, "std_logic"));
				comp.Ports.Add(new SimplifiedPortInfo("douta", PortDirection.Out, "std_logic_vector(READ_DATA_WIDTH_A-1 downto 0)"));
				comp.Ports.Add(new SimplifiedPortInfo("sbiterra", PortDirection.Out, "std_logic"));
			}

			comp.Ports.Add(new SimplifiedPortInfo("addra", PortDirection.In, "std_logic_vector(ADDR_WIDTH_A-1 downto 0)"));
			if (ram)
				comp.Ports.Add(new SimplifiedPortInfo("addrb", PortDirection.In, "std_logic_vector(ADDR_WIDTH_B-1 downto 0)"));
			comp.Ports.Add(new SimplifiedPortInfo("clka", PortDirection.In, "std_logic"));
			if (ram)
			{
				comp.Ports.Add(new SimplifiedPortInfo("clkb", PortDirection.In, "std_logic"));
				comp.Ports.Add(new SimplifiedPortInfo("dina", PortDirection.In, "std_logic_vector(WRITE_DATA_WIDTH_A-1 downto 0)"));
			}
			comp.Ports.Add(new SimplifiedPortInfo("ena", PortDirection.In, "std_logic"));
			if (ram)
				comp.Ports.Add(new SimplifiedPortInfo("enb", PortDirection.In, "std_logic"));

			comp.Ports.Add(new SimplifiedPortInfo("injectdbiterra", PortDirection.In, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("injectsbiterra", PortDirection.In, "std_logic"));
			if (ram)
			{
				comp.Ports.Add(new SimplifiedPortInfo("regceb", PortDirection.In, "std_logic"));
				comp.Ports.Add(new SimplifiedPortInfo("rstb", PortDirection.In, "std_logic"));
			}
			else
			{
				comp.Ports.Add(new SimplifiedPortInfo("regcea", PortDirection.In, "std_logic"));
				comp.Ports.Add(new SimplifiedPortInfo("rsta", PortDirection.In, "std_logic"));
			}
			comp.Ports.Add(new SimplifiedPortInfo("sleep", PortDirection.In, "std_logic"));
			if (ram)
				comp.Ports.Add(new SimplifiedPortInfo("wea", PortDirection.In, "std_logic_vector((WRITE_DATA_WIDTH_A/BYTE_WRITE_WIDTH_A)-1 downto 0)"));
			return comp;
		}

		#endregion
	}
}
