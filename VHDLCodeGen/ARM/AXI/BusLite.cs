//********************************************************************************************************************************
// Filename:    BusLite.cs
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

namespace VHDLCodeGen.ARM.AXI
{
	/// <summary>
	///   Contains information about an AXI Lite bus.
	/// </summary>
	public abstract class BusLite
	{
		#region Properties

		/// <summary>
		///   Gets the width of the registers contained in the AXI Lite interface.
		/// </summary>
		public int RegisterWidth
		{
			get
			{
				return 32;
			}
		}

		/// <summary>
		///   Gets the width of the registers contained in the AXI Lite interface in bytes.
		/// </summary>
		public int RegisterByteWidth
		{
			get
			{
				return RegisterWidth / 8;
			}
		}

		/// <summary>
		///   Gets the index of the least significant bit of the word address. Used to convert byte addressing to word addressing.
		/// </summary>
		/// <remarks>For 32-bit registers it will return 2. For 64-bit register it will return 3.</remarks>
		public int AddressLSB
		{
			get
			{
				if (RegisterWidth == 32)
					return 2;
				return 3;
			}
		}

		/// <summary>
		///   Gets the unique name of the AXI Lite bus.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets a description of the AXI Lite bus. Can be null or empty.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		///   Gets or sets whether to include the AXI Lite bus clock when generating ports for the module.
		/// </summary>
		public bool IncludeClock { get; set; }

		/// <summary>
		///   Gets or sets whether to include the AXI lite bus reset when generating ports for the module.
		/// </summary>
		public bool IncludeReset { get; set; }

		/// <summary>
		///   Gets the type of interface the bus is providing (master, slave or monitor).
		/// </summary>
		public InterfaceType BusType { get; private set; }

		/// <summary>
		///   Gets the attribute name of the bus data width.
		/// </summary>
		public string DataWidthAttributeName { get; private set; }

		/// <summary>
		///   Gets the attribute name of the address width.
		/// </summary>
		public string AddressWidthAttributeName { get; private set; }

		/// <summary>
		///   Gets the minimum address width required to contain the registers and memories specified.
		/// </summary>
		public int MinAddressWidth { get; private set; }

		/// <summary>
		///   Gets the maximum address width allowed for the AXI Lite interface.
		/// </summary>
		public int MaxAddressWidth { get; private set; }

		/// <summary>
		///   True if this interface is a single bus in the module, false if it is one of many.
		/// </summary>
		public bool SingleBus { get; private set; }

		#endregion

		#region SignalNames

		/// <summary>
		///   Gets the name of the clock associated with the bus interface.
		/// </summary>
		public string ClockName { get; private set; }

		/// <summary>
		///   Gets the name of the reset associated with the bus interface.
		/// </summary>
		public string ResetName { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="BusLite"/> object.
		/// </summary>
		/// <param name="type"><see cref="InterfaceType"/> of the bus.</param>
		/// <param name="minAddressWidth">Minimum address width of the AXI Lite interface.</param>
		/// <param name="name">Name of the interface. If null, then the name will be generated based on the interface type.</param>
		/// <param name="singleBus">True if this is the only AXI-Lite bus in the module. False otherwise.</param>
		public BusLite(InterfaceType type, int minAddressWidth, string name = null, bool singleBus = true)
		{
			if (!Enum.IsDefined(typeof(InterfaceType), type))
				throw new ArgumentException("The AXI bus type was not recognized as a valid type.", nameof(type));
			if (minAddressWidth < 1 || minAddressWidth > 32)
				throw new ArgumentException($"The minimum address width specified ({minAddressWidth}) is less than 1 or greater than 32.", nameof(minAddressWidth));

			MinAddressWidth = minAddressWidth;
			MaxAddressWidth = 32;
			BusType = type;
			SingleBus = singleBus;
			if (string.IsNullOrEmpty(name))
				Name = AXI.Utility.GetName(BusType);
			else
				Name = name;

			DataWidthAttributeName = Utility.GetDataWidthAttributeName(Name);
			AddressWidthAttributeName = Utility.GetAddressWidthAttributeName(Name);
			if (SingleBus)
			{
				ClockName = Utility.GetName(Signal.ACLK, BusType, string.Empty);
				ResetName = Utility.GetName(Signal.ARESETN, BusType, string.Empty);
			}
			else
			{
				ClockName = Utility.GetName(Signal.ACLK, BusType, Name);
				ResetName = Utility.GetName(Signal.ARESETN, BusType, Name);
			}
		}

		/// <summary>
		///   Generates the required logic for the bus and adds the logic to the specified module.
		/// </summary>
		/// <param name="mod"><see cref="ModuleInfo"/> to add the bus to.</param>
		/// <exception cref="ArgumentNullException"><paramref name="mod"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">
		///   Overlaps occurred in the <see cref="Memories"/> and/or <see cref="Registers"/>.
		/// </exception>
		public abstract void Generate(ModuleInfo mod);

		/// <summary>
		///   Adds the entity components (ports and generics) for the AXI Lite bus.
		/// </summary>
		/// <param name="entity"><see cref="EntityInfo"/> object to add the ports or generics to.</param>
		/// <param name="includePorts">True if the ports should be added, false if they should not.</param>
		/// <param name="includeGenerics">True if the generics should be added, false if they should not.</param>
		/// <exception cref="ArgumentNullException"><paramref name="entity"/> is a null reference.</exception>
		public void GenerateEntityComponents(EntityInfo entity, bool includePorts = true, bool includeGenerics = true)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			if (!includePorts && !includeGenerics)
				return;

			if (includeGenerics)
			{
				entity.Generics.Add(new GenericInfo(AddressWidthAttributeName, "Number of bits on the address buses.", $"integer range {MinAddressWidth} to {MaxAddressWidth}", MinAddressWidth.ToString()));
				entity.Generics.Add(new GenericInfo(DataWidthAttributeName, "Number of bits on the data buses.", "integer range 32 to 32", "32"));
			}

			if (includePorts)
			{
				if (IncludeClock)
					entity.Ports.Add(Utility.GetPortInfo(Signal.ACLK, BusType, Name, SingleBus));
				if (IncludeReset)
					entity.Ports.Add(Utility.GetPortInfo(Signal.ARESETN, BusType, Name, SingleBus));

				// Write Address Channel
				entity.Ports.Add(Utility.GetPortInfo(Signal.AWADDR, BusType, Name, SingleBus));
				entity.Ports.Add(Utility.GetPortInfo(Signal.AWVALID, BusType, Name, SingleBus));
				entity.Ports.Add(Utility.GetPortInfo(Signal.AWREADY, BusType, Name, SingleBus));
				entity.Ports.Add(Utility.GetPortInfo(Signal.AWPROT, BusType, Name, SingleBus));

				// Write Data Channel
				entity.Ports.Add(Utility.GetPortInfo(Signal.WDATA, BusType, Name, SingleBus));
				entity.Ports.Add(Utility.GetPortInfo(Signal.WVALID, BusType, Name, SingleBus));
				entity.Ports.Add(Utility.GetPortInfo(Signal.WREADY, BusType, Name, SingleBus));
				entity.Ports.Add(Utility.GetPortInfo(Signal.WSTRB, BusType, Name, SingleBus));

				// Write Response Channel
				entity.Ports.Add(Utility.GetPortInfo(Signal.BRESP, BusType, Name, SingleBus));
				entity.Ports.Add(Utility.GetPortInfo(Signal.BVALID, BusType, Name, SingleBus));
				entity.Ports.Add(Utility.GetPortInfo(Signal.BREADY, BusType, Name, SingleBus));

				// Read Address Channel
				entity.Ports.Add(Utility.GetPortInfo(Signal.ARADDR, BusType, Name, SingleBus));
				entity.Ports.Add(Utility.GetPortInfo(Signal.ARVALID, BusType, Name, SingleBus));
				entity.Ports.Add(Utility.GetPortInfo(Signal.ARREADY, BusType, Name, SingleBus));
				entity.Ports.Add(Utility.GetPortInfo(Signal.ARPROT, BusType, Name, SingleBus));

				// Read Data Channel
				entity.Ports.Add(Utility.GetPortInfo(Signal.RDATA, BusType, Name, SingleBus));
				entity.Ports.Add(Utility.GetPortInfo(Signal.RVALID, BusType, Name, SingleBus));
				entity.Ports.Add(Utility.GetPortInfo(Signal.RREADY, BusType, Name, SingleBus));
				entity.Ports.Add(Utility.GetPortInfo(Signal.RRESP, BusType, Name, SingleBus));
			}
		}

		/// <summary>
		///   Adds the AXI bus declared types to the module.
		/// </summary>
		/// <param name="mod"><see cref="ModuleInfo"/> object to add the declared types to.</param>
		/// <param name="responses"><see cref="Response"/> types to add.</param>
		/// <exception cref="ArgumentNullException"><paramref name="mod"/> is a null reference.</exception>
		public void GenerateDeclaredTypes(ModuleInfo mod, params Response[] responses)
		{
			if (mod == null)
				throw new ArgumentNullException(nameof(mod));

			foreach (Response response in responses)
			{
				mod.DeclaredTypes.Add(new DeclarationInfo(
					DeclarationType.Constant,
					$"C_{Enum.GetName(typeof(Response), response)}",
					"std_logic_vector(1 downto 0)",
					Utility.GetDescription(response),
					VHDLUtility.GetBitString((ulong)response, 2, true),
					Utility.GetClarification(response)
				));
			}
		}

		/// <summary>
		///   Gets the name of the specified signal on this bus.
		/// </summary>
		/// <param name="sig"><see cref="Signal"/> on this bus to obtain the name for.</param>
		/// <returns>Name of the signal on this bus.</returns>
		public string GetName(Signal sig)
		{
			return Utility.GetName(sig, BusType, Name);
		}

		#endregion
	}
}
