//********************************************************************************************************************************
// Filename:    Utility.cs
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
	///   Contains various utility methods for working with AXI bus interfaces.
	/// </summary>
	public static class Utility
	{
		#region Methods

		/// <summary>
		///   Gets the address width attribute name based on the bus name.
		/// </summary>
		/// <param name="busName">Name of the bus.</param>
		/// <returns>Generated name of the attribute.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="busName"/> is null or whitespace.</exception>
		public static string GetAddressWidthAttributeName(string busName)
		{
			if (string.IsNullOrWhiteSpace(busName))
				throw new ArgumentNullException(nameof(busName));
			return $"C_{busName}_ADDR_WIDTH";
		}

		/// <summary>
		///   Gets additional clarification notes on the AXI bus response type.
		/// </summary>
		/// <param name="res"><see cref="Response"/> type to obtain the description of.</param>
		/// <returns>String containing clarification of the specified response type.</returns>
		/// <exception cref="ArgumentException"><paramref name="res"/> is not recognized as a valid type.</exception>
		public static string GetClarification(Response res)
		{
			if (!Enum.IsDefined(typeof(Response), res))
				throw new ArgumentException("The response type was not recognized as a valid type.", nameof(res));

			switch (res)
			{
				case Response.OKAY:
					return "Can also indicate that an exclusive access has failed.";
				case Response.EXOKAY:
					return string.Empty;
				case Response.SLVERR:
					return "Examples of slave error conditions are: FIFO or buffer overrun or underrun condition, write access attempted to read-only location, timeout condition in the slave, or access attempted to a disabled or powered-down function.";
				case Response.DECERR:
					return string.Empty;
				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		///   Gets the data width attribute name based on the bus name.
		/// </summary>
		/// <param name="busName">Name of the bus.</param>
		/// <returns>Generated name of the attribute.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="busName"/> is null or whitespace.</exception>
		public static string GetDataWidthAttributeName(string busName)
		{
			if (string.IsNullOrWhiteSpace(busName))
				throw new ArgumentNullException(nameof(busName));
			return $"C_{busName}_DATA_WIDTH";
		}

		/// <summary>
		/// Gets the default value of the specified signal.
		/// </summary>
		/// <param name="sig"><see cref="Signal"/> to obtain the default value of.</param>
		/// <param name="type"><see cref="InterfaceType"/> describing the type of interface the signal belongs.</param>
		/// <returns>String containing the default value.</returns>
		/// <exception cref="ArgumentException"><paramref name="sig"/> or <paramref name="type"/> is not recognized as valid.</exception>
		public static string GetDefaultValue(Signal sig, InterfaceType type)
		{
			if (!Enum.IsDefined(typeof(Signal), sig))
				throw new ArgumentException("The signal type was not recognized as a valid signal.", nameof(sig));
			if (!Enum.IsDefined(typeof(InterfaceType), type))
				throw new ArgumentException("The interface type was not recognized as a valid type.", nameof(type));

			switch (sig)
			{
				case Signal.ACLK:
				case Signal.ARESETN:
				case Signal.AWVALID:
				case Signal.AWREADY:
				case Signal.AWADDR:
				case Signal.AWPROT:
				case Signal.WDATA:
				case Signal.WVALID:
				case Signal.WREADY:
				case Signal.BVALID:
				case Signal.BREADY:
				case Signal.BID:
				case Signal.ARADDR:
				case Signal.ARPROT:
				case Signal.ARVALID:
				case Signal.ARREADY:
				case Signal.RREADY:
				case Signal.RVALID:
				case Signal.RID:
				case Signal.RDATA:
					return null;
				case Signal.AWID:
				case Signal.AWREGION:
				case Signal.AWLEN:
				case Signal.AWCACHE:
				case Signal.AWQOS:
				case Signal.AWUSER:
				case Signal.WID:
				case Signal.WUSER:
				case Signal.BUSER:
				case Signal.ARID:
				case Signal.ARREGION:
				case Signal.ARLEN:
				case Signal.ARCACHE:
				case Signal.ARQOS:
				case Signal.ARUSER:
				case Signal.RUSER:
					if (type != InterfaceType.Master)
						return "(others => '0')";
					else
						return null;
				case Signal.AWSIZE:
				case Signal.ARSIZE:
					if (type != InterfaceType.Master)
						return VHDLUtility.GetBitString(4, 3, true, 1); // '010' 4 bytes for a 32-bit wide bus.
					else
						return null;
				case Signal.AWBURST:
				case Signal.ARBURST:
					if (type != InterfaceType.Master)
						return VHDLUtility.GetBitString((ulong)BurstType.Increment, 3, true);
					else
						return null;
				case Signal.AWLOCK:
				case Signal.ARLOCK:
					if (type != InterfaceType.Master)
						return "'0'";
					else
						return null;
				case Signal.WSTRB:
					if (type != InterfaceType.Master)
						return "(others => '1')";
					else
						return null;
				case Signal.WLAST:
				case Signal.RLAST:
					if (type != InterfaceType.Master)
						return "'1'";
					else
						return null;
				case Signal.BRESP:
				case Signal.RRESP:
					if (type == InterfaceType.Master)
						return VHDLUtility.GetBitString((ulong)Response.OKAY, 2, true);
					else
						return null;
				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		///   Gets a description of the AXI bus response type.
		/// </summary>
		/// <param name="res"><see cref="Response"/> type to obtain the description of.</param>
		/// <returns>String containing a description of the specified response type.</returns>
		/// <exception cref="ArgumentException"><paramref name="res"/> is not recognized as a valid type.</exception>
		public static string GetDescription(Response res)
		{
			if (!Enum.IsDefined(typeof(Response), res))
				throw new ArgumentException("The response type was not recognized as a valid type.", nameof(res));

			switch(res)
			{
				case Response.OKAY:
					return "Normal access success. Indicates that a normal access has been successful.";
				case Response.EXOKAY:
					return "Exclusive access okay. Indicates that either the read or write portion of an exclusive access has been successful.";
				case Response.SLVERR:
					return "Slave error. Used when the access has reached the slave successfully, but the slave wishes to return an error condition to the originating master.";
				case Response.DECERR:
					return "Decode error. Generated, typically by an interconnect component, to indicate that there is no slave at the transaction address.";
				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets the direction of the signal based on the interface type.
		/// </summary>
		/// <param name="sig"><see cref="Signal"/> to obtain the direction for.</param>
		/// <param name="type"><see cref="InterfaceType"/> of the bus.</param>
		/// <returns><see cref="PortDirection"/> containing the direction of the signal on that interface type.</returns>
		/// <exception cref="ArgumentException"><paramref name="sig"/> or <paramref name="type"/> is not recognized as valid.</exception>
		public static PortDirection GetDirection(Signal sig, InterfaceType type)
		{
			if (!Enum.IsDefined(typeof(Signal), sig))
				throw new ArgumentException("The signal type was not recognized as a valid signal.", nameof(sig));
			if (!Enum.IsDefined(typeof(InterfaceType), type))
				throw new ArgumentException("The interface type was not recognized as a valid type.", nameof(type));

			if (type == InterfaceType.Monitor)
				return PortDirection.In;

			switch (sig)
			{
				case Signal.ACLK:
				case Signal.ARESETN:
					return PortDirection.In;
				case Signal.AWID:
				case Signal.AWADDR:
				case Signal.AWLEN:
				case Signal.AWSIZE:
				case Signal.AWBURST:
				case Signal.AWLOCK:
				case Signal.AWCACHE:
				case Signal.AWPROT:
				case Signal.AWQOS:
				case Signal.AWREGION:
				case Signal.AWUSER:
				case Signal.AWVALID:
				case Signal.WID:
				case Signal.WDATA:
				case Signal.WSTRB:
				case Signal.WLAST:
				case Signal.WUSER:
				case Signal.WVALID:
				case Signal.BREADY:
				case Signal.ARID:
				case Signal.ARADDR:
				case Signal.ARLEN:
				case Signal.ARSIZE:
				case Signal.ARBURST:
				case Signal.ARLOCK:
				case Signal.ARCACHE:
				case Signal.ARPROT:
				case Signal.ARQOS:
				case Signal.ARREGION:
				case Signal.ARUSER:
				case Signal.ARVALID:
				case Signal.RREADY:
					if (type == InterfaceType.Master)
						return PortDirection.Out;
					else
						return PortDirection.In;
				case Signal.AWREADY:
				case Signal.WREADY:
				case Signal.BID:
				case Signal.BRESP:
				case Signal.BUSER:
				case Signal.BVALID:
				case Signal.ARREADY:
				case Signal.RID:
				case Signal.RDATA:
				case Signal.RRESP:
				case Signal.RLAST:
				case Signal.RUSER:
				case Signal.RVALID:
					if (type == InterfaceType.Master)
						return PortDirection.In;
					else
						return PortDirection.Out;
				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		///   Gets the name of the bus based on the interface type.
		/// </summary>
		/// <param name="type"><see cref="InterfaceType"/> of the bus interface the name will be associated with.</param>
		/// <returns>String containing the name of the bus.</returns>
		/// <exception cref="ArgumentException"><paramref name="type"/> is not recognized as a valid type.</exception>
		public static string GetName(InterfaceType type)
		{
			if (!Enum.IsDefined(typeof(InterfaceType), type))
				throw new ArgumentException("The interface type was not recognized as a valid type.", nameof(type));

			if (type == InterfaceType.Master)
				return "M_AXI";
			else if (type == InterfaceType.Slave)
				return "S_AXI";
			else
				return "MON_AXI";
		}

		/// <summary>
		///   Gets the name of the signal.
		/// </summary>
		/// <param name="sig"><see cref="Signal"/> to obtain the name of.</param>
		/// <param name="type"><see cref="InterfaceType"/> of the bus interface the name will be associated with.</param>
		/// <param name="baseName">
		///   Base name of the bus. If this value is null then the base name will be pulled from <see cref="GetName(InterfaceType)"/> using
		///   <paramref name="type"/>. If this value is empty then no basename will be added to the signal.
		/// </param>
		/// <returns>String containing the name of the signal combined with it's corresponding bus base name.</returns>
		/// <remarks>To avoid any base name (Ex: WDATA -> 'WDATA') then set <paramref name="baseName"/> to an empty string.</remarks>
		/// <exception cref="ArgumentException"><paramref name="sig"/> or <paramref name="type"/> is not recognized as a valid.</exception>
		public static string GetName(Signal sig, InterfaceType type, string baseName = null)
		{
			if (!Enum.IsDefined(typeof(Signal), sig))
				throw new ArgumentException("The signal type was not recognized as a valid signal.", nameof(sig));
			if (!Enum.IsDefined(typeof(InterfaceType), type))
				throw new ArgumentException("The interface type was not recognized as a valid type.", nameof(type));

			if (baseName == null)
				baseName = GetName(type);

			if(baseName == string.Empty)
				return $"{Enum.GetName(typeof(Signal), sig)}";
			return $"{baseName}_{Enum.GetName(typeof(Signal), sig)}";
		}

		/// <summary>
		///   Gets the <see cref="PortInfo"/> object based on the specified signal.
		/// </summary>
		/// <param name="sig"><see cref="Signal"/> to obtain the port for.</param>
		/// <param name="singleBus">True if the module only has one AXI bus interface, calse if it has more than one.</param>
		/// <returns><see cref="PortInfo"/> object containing all the information about the generated port.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="busName"/> is null or whitespace.</exception>
		public static PortInfo GetPortInfo(Signal sig, InterfaceType type, string busName, bool singleBus = true)
		{
			if (string.IsNullOrWhiteSpace(busName))
				throw new ArgumentNullException(nameof(busName));

			string variableWidthString = null;
			if (sig == Signal.ARADDR || sig == Signal.AWADDR)
				variableWidthString = GetAddressWidthAttributeName(busName);
			if (sig == Signal.RDATA || sig == Signal.WDATA || sig == Signal.WSTRB)
				variableWidthString = GetDataWidthAttributeName(busName);

			string name = busName;
			if ((sig == Signal.ACLK || sig == Signal.ARESETN) && singleBus)
			{
				// Don't add the S_AXI_ in front of the ACLK and ARESETN signals if they are the only bus interface in the module.
				name = string.Empty;
			}

			return new PortInfo
			(
				Utility.GetName(sig, type, name),
				Utility.GetSignalDescription(sig),
				Utility.GetDirection(sig, type),
				Utility.GetType(sig, variableWidthString),
				Utility.GetDefaultValue(sig, type)
			);
		}

		/// <summary>
		/// Gets a description of the AXI bus signal.
		/// </summary>
		/// <param name="sig"><see cref="Signal"/> to obtain the description of.</param>
		/// <returns>String containing a description of the specified signal type.</returns>
		/// <exception cref="ArgumentException"><paramref name="sig"/> is not recognized as a valid signal.</exception>
		public static string GetSignalDescription(Signal sig)
		{
			if (!Enum.IsDefined(typeof(Signal), sig))
				throw new ArgumentException("The signal was not recognized as a valid signal type.", nameof(sig));

			switch (sig)
			{
				case Signal.ACLK:
					return "Global clock signal. Synchronous signals are sampled on the rising edge of the global clock.";
				case Signal.ARESETN:
					return "Global reset signal. This signal is active-LOW.";
				case Signal.AWID:
					return "Identification tag for a write transaction.";
				case Signal.AWADDR:
					return "The address of the first transfer in a write transaction.";
				case Signal.AWLEN:
					return "Length, the exact number of data transfers in a write transaction. This information determines the number of data transfers associated with the address.";
				case Signal.AWSIZE:
					return "Size, the number of bytes in each data transfer in a write transaction.";
				case Signal.AWBURST:
					return "Burst type, indicates how address changes between each transfer in a write transaction.";
				case Signal.AWLOCK:
					return "Provides information about the atomic characteristics of a write transaction.";
				case Signal.AWCACHE:
					return "Indicates how a write transaction is required to progress through a system.";
				case Signal.AWPROT:
					return "Protection attributes of a write transaction: privilege, security level, and access type.";
				case Signal.AWQOS:
					return "Quality of Service identifier for a write transaction.";
				case Signal.AWREGION:
					return "Region indicator for a write transaction.";
				case Signal.AWUSER:
					return "User-defined extension for the write address channel.";
				case Signal.AWVALID:
					return "Indicates that the write address channel signals are valid.";
				case Signal.AWREADY:
					return "Indicates that a transfer on the write address channel can be accepted.";
				case Signal.WID:
					return "The ID tag of the write data transfer.";
				case Signal.WDATA:
					return "Write data.";
				case Signal.WSTRB:
					return "Write strobes, indicate which byte lanes hold valid data.";
				case Signal.WLAST:
					return "Indicates whether this is the last data transfer in a write transaction.";
				case Signal.WUSER:
					return "User-defined extension for the write data channel.";
				case Signal.WVALID:
					return "Indicates that the write data channel signals are valid.";
				case Signal.WREADY:
					return "Indicates that a transfer on the write data channel can be accepted.";
				case Signal.BID:
					return "Identification tag for a write response.";
				case Signal.BRESP:
					return "Write response, indicates the status of a write transaction.";
				case Signal.BUSER:
					return "User-defined extension for the write response channel.";
				case Signal.BVALID:
					return "Indicates that the write response channel signals are valid.";
				case Signal.BREADY:
					return "Indicates that a transfer on the write response channel can be accepted.";
				case Signal.ARID:
					return "Identification tag for a read transaction.";
				case Signal.ARADDR:
					return "The address of the first transfer in a read transaction.";
				case Signal.ARLEN:
					return "Length, the exact number of data transfers in a read transaction.";
				case Signal.ARSIZE:
					return "Size, the number of bytes in each data transfer in a read transaction.";
				case Signal.ARBURST:
					return "Burst type, indicates how address changes between each transfer in a read transaction.";
				case Signal.ARLOCK:
					return "Provides information about the atomic characteristics of a read transaction.";
				case Signal.ARCACHE:
					return "Indicates how a read transaction is required to progress through a system.";
				case Signal.ARPROT:
					return "Protection attributes of a read transaction: privilege, security level, and access type.";
				case Signal.ARQOS:
					return "Quality of Service identifier for a read transaction.";
				case Signal.ARREGION:
					return "Region indicator for a read transaction.";
				case Signal.ARUSER:
					return "User-defined extension for the read address channel.";
				case Signal.ARVALID:
					return "Indicates that the read address channel signals are valid.";
				case Signal.ARREADY:
					return "Indicates that a transfer on the read address channel can be accepted.";
				case Signal.RID:
					return "Identification tag for read data and response.";
				case Signal.RDATA:
					return "Read data.";
				case Signal.RRESP:
					return "Read response, indicates the status of a read transfer.";
				case Signal.RLAST:
					return "Indicates whether this is the last data transfer in a read transaction.";
				case Signal.RUSER:
					return "User-defined extension for the read data channel.";
				case Signal.RVALID:
					return "Indicates that the read data channel signals are valid.";
				case Signal.RREADY:
					return "Indicates that a transfer on the read data channel can be accepted.";
				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets the type associated with the specified signal.
		/// </summary>
		/// <param name="sig">Signal to obtain the type of.</param>
		/// <param name="variableWidthString">
		/// If the Signal can be dynamic in size, then the variable width string is passed in using this parameter. This parameter should represent
		/// the size of the signal so a '-1' is added by this method. Ex: if 'C_AXI_DATA_WIDTH' is passed in then the string returned for WDATA would
		/// be 'std_logic_vector(C_AXI_DATA_WIDTH-1 downto 0)'. If this signal is null, then a default value will be used. IDs -> 'C_ID_WIDTH', AxADDR
		/// -> 'C_AXI_ADDR_WIDTH', USERs -> 'C_USER_WIDTH', xDATA -> 'C_AXI_DATA_WIDTH', and WSTRB -> 'C_AXI_DATA_WIDTH'.
		/// </param>
		/// <returns>The type string associated with the signal.</returns>
		/// <exception cref="ArgumentException"><paramref name="sig"/> is not recognized as a valid signal.</exception>
		public static string GetType(Signal sig, string variableWidthString = null)
		{
			if (!Enum.IsDefined(typeof(Signal), sig))
				throw new ArgumentException("The signal was not recognized as a valid signal type.", nameof(sig));

			switch (sig)
			{
				case Signal.AWID:
				case Signal.WID:
				case Signal.BID:
				case Signal.ARID:
				case Signal.RID:
					if (variableWidthString == null)
						variableWidthString = "C_ID_WIDTH";
					return $"std_logic_vector({variableWidthString}-1 downto 0)";
				case Signal.AWADDR:
				case Signal.ARADDR:
					if (variableWidthString == null)
						variableWidthString = "C_AXI_ADDR_WIDTH";
					return $"std_logic_vector({variableWidthString}-1 downto 0)";
				case Signal.AWLEN:
				case Signal.ARLEN:
					return "std_logic_vector(7 downto 0)";
				case Signal.AWSIZE:
				case Signal.ARSIZE:
					return "std_logic_vector(2 downto 0)";
				case Signal.AWBURST:
				case Signal.ARBURST:
					return "std_logic_vector(1 downto 0)";
				case Signal.AWLOCK:
				case Signal.ARLOCK:
					//return "std_logic_vector(1 downto 0)"; -- AXI3
					return "std_logic";
				case Signal.AWCACHE:
				case Signal.ARCACHE:
					return "std_logic_vector(3 downto 0)";
				case Signal.AWPROT:
				case Signal.ARPROT:
					return "std_logic_vector(2 downto 0)";
				case Signal.AWQOS:
				case Signal.ARQOS:
					return "std_logic_vector(3 downto 0)";
				case Signal.AWREGION:
				case Signal.ARREGION:
					return "std_logic_vector(3 downto 0)";
				case Signal.AWUSER:
				case Signal.ARUSER:
				case Signal.WUSER:
				case Signal.BUSER:
				case Signal.RUSER:
					if (variableWidthString == null)
						variableWidthString = "C_USER_WIDTH";
					return $"std_logic_vector({variableWidthString}-1 downto 0)";
				case Signal.ACLK:
				case Signal.ARESETN:
				case Signal.AWVALID:
				case Signal.AWREADY:
				case Signal.ARVALID:
				case Signal.ARREADY:
				case Signal.WLAST:
				case Signal.WVALID:
				case Signal.WREADY:
				case Signal.BVALID:
				case Signal.BREADY:
				case Signal.RLAST:
				case Signal.RVALID:
				case Signal.RREADY:
					return "std_logic";
				case Signal.WDATA:
				case Signal.RDATA:
					if (variableWidthString == null)
						variableWidthString = "C_AXI_DATA_WIDTH";
					return $"std_logic_vector({variableWidthString}-1 downto 0)";
				case Signal.WSTRB:
					if (variableWidthString == null)
						variableWidthString = "C_AXI_DATA_WIDTH";
					return $"std_logic_vector(({variableWidthString}/8)-1 downto 0)";
				case Signal.BRESP:
				case Signal.RRESP:
					return $"std_logic_vector(1 downto 0)";
				default:
					throw new NotImplementedException();
			}
		}

		#endregion
	}
}
