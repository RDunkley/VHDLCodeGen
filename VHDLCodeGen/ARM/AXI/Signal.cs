//********************************************************************************************************************************
// Filename:    Signal.cs
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

namespace VHDLCodeGen.ARM.AXI
{
	/// <summary>
	/// Enumerates the various signals in an AXI bus.
	/// </summary>
	public enum Signal
	{
		#region Global

		/// <summary>
		///  Global clock signal. Synchronous signals are sampled on the rising edge of the	global clock. 
		/// </summary>
		ACLK,

		/// <summary>
		///   Global reset signal. This signal is active-LOW.
		/// </summary>
		ARESETN,

		#endregion

		#region Write Address Channel

		/// <summary>
		///   Identification tag for a write transaction.
		/// </summary>
		AWID,

		/// <summary>
		///   The address of the first transfer in a write transaction.
		/// </summary>
		AWADDR,

		/// <summary>
		///    Length, the exact number of data transfers in a write transaction. This information determines the number of data transfers associated with the address.
		/// </summary>
		/// <remarks>This changes between AXI3 and AXI4.</remarks>
		AWLEN,

		/// <summary>
		///   Size, the number of bytes in each data transfer in a write transaction.
		/// </summary>
		AWSIZE,

		/// <summary>
		///   Burst type, indicates how address changes between each transfer in a write transaction.
		/// </summary>
		AWBURST,

		/// <summary>
		///    Provides information about the atomic characteristics of a write transaction.
		/// </summary>
		/// <remarks>This changes between AXI3 and AXI4.</remarks>
		AWLOCK,

		/// <summary>
		///   Indicates how a write transaction is required to progress through a system.
		/// </summary>
		AWCACHE,

		/// <summary>
		///   Protection attributes of a write transaction: privilege, security level, and access type.
		/// </summary>
		AWPROT,

		/// <summary>
		///    Quality of Service identifier for a write transaction.
		/// </summary>
		/// <remarks>Not implemented in AXI3.</remarks>
		AWQOS,

		/// <summary>
		///   Region indicator for a write transaction.
		/// </summary>
		/// <remarks>Not implemented in AXI3.</remarks>
		AWREGION,

		/// <summary>
		///    User-defined extension for the write address channel.
		/// </summary>
		/// <remarks>Not implemented in AXI3.</remarks>
		AWUSER,

		/// <summary>
		///   Indicates that the write address channel signals are valid.
		/// </summary>
		AWVALID,

		/// <summary>
		///   Indicates that a transfer on the write address channel can be accepted.
		/// </summary>
		AWREADY,

		#endregion

		#region Write Data Channel

		/// <summary>
		///   The ID tag of the write data transfer.
		/// </summary>
		/// <remarks>Implemented in AXI3 only.</remarks>
		WID,

		/// <summary>
		///   Write data.
		/// </summary>
		WDATA,

		/// <summary>
		///   Write strobes, indicate which byte lanes hold valid data.
		/// </summary>
		WSTRB,

		/// <summary>
		/// Indicates whether this is the last data transfer in a write transaction.
		/// </summary>
		WLAST,

		/// <summary>
		/// User-defined extension for the write data channel.
		/// </summary>
		/// <remarks>Not implemented in AXI3.</remarks>
		WUSER,

		/// <summary>
		/// Indicates that the write data channel signals are valid.
		/// </summary>
		WVALID,

		/// <summary>
		/// Indicates that a transfer on the write data channel can be accepted.
		/// </summary>
		WREADY,

		#endregion

		#region Write Response Channel

		/// <summary>
		/// Identification tag for a write response.
		/// </summary>
		BID,

		/// <summary>
		/// Write response, indicates the status of a write transaction.
		/// </summary>
		BRESP,

		/// <summary>
		/// User-defined extension for the write response channel.
		/// </summary>
		/// <remarks>Not implemented in AXI3.</remarks>
		BUSER,

		/// <summary>
		///  Indicates that the write response channel signals are valid.
		/// </summary>
		BVALID,

		/// <summary>
		/// Indicates that a transfer on the write response channel can be accepted.
		/// </summary>
		BREADY,

		#endregion

		#region Read Address Channel

		/// <summary>
		/// Identification tag for a read transaction.
		/// </summary>
		ARID,

		/// <summary>
		/// The address of the first transfer in a read transaction.
		/// </summary>
		ARADDR,

		/// <summary>
		/// Length, the exact number of data transfers in a read transaction.
		/// </summary>
		/// <remarks>This changes between AXI3 and AXI4.</remarks>
		ARLEN,

		/// <summary>
		/// Size, the number of bytes in each data transfer in a read transaction.
		/// </summary>
		ARSIZE,

		/// <summary>
		///  Burst type, indicates how address changes between each transfer in a read transaction.
		/// </summary>
		ARBURST,

		/// <summary>
		/// Provides information about the atomic characteristics of a read transaction.
		/// </summary>
		/// <remarks>This changes between AXI3 and AXI4.</remarks>
		ARLOCK,

		/// <summary>
		/// Indicates how a read transaction is required to progress through a system.
		/// </summary>
		ARCACHE,

		/// <summary>
		/// Protection attributes of a read transaction: privilege, security level, and access type.
		/// </summary>
		ARPROT,

		/// <summary>
		/// Quality of Service identifier for a read transaction.
		/// </summary>
		/// <remarks>Not implemented in AXI3.</remarks>
		ARQOS,

		/// <summary>
		/// Region indicator for a read transaction.
		/// </summary>
		/// <remarks>Not implemented in AXI3.</remarks>
		ARREGION,

		/// <summary>
		///  User-defined extension for the read address channel.
		/// </summary>
		/// <remarks>Not implemented in AXI3.</remarks>
		ARUSER,

		/// <summary>
		/// Indicates that the read address channel signals are valid.
		/// </summary>
		ARVALID,

		/// <summary>
		///  Indicates that a transfer on the read address channel can be accepted.
		/// </summary>
		ARREADY,

		#endregion

		#region Read Data Channel

		/// <summary>
		/// Identification tag for read data and response.
		/// </summary>
		RID,

		/// <summary>
		/// Read data.
		/// </summary>
		RDATA,

		/// <summary>
		///  Read response, indicates the status of a read transfer.
		/// </summary>
		RRESP,

		/// <summary>
		/// Indicates whether this is the last data transfer in a read transaction.
		/// </summary>
		RLAST,

		/// <summary>
		/// User-defined extension for the read data channel.
		/// </summary>
		/// <remarks>Not implemented in AXI3.</remarks>
		RUSER,

		/// <summary>
		/// Indicates that the read data channel signals are valid.
		/// </summary>
		RVALID,

		/// <summary>
		/// Indicates that a transfer on the read data channel can be accepted.
		/// </summary>
		RREADY,

		#endregion
	}
}
