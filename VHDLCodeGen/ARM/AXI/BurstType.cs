//********************************************************************************************************************************
// Filename:    BurstType.cs
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
	/// Enumerates the various types of bursts that can be done on an AXI bus.
	/// </summary>
	public enum BurstType
	{
		/// <summary>
		/// In a fixed burst: The address is the same for every transfer in the burst and the byte lanes
		/// that are valid are constant for all beats in the burst. However, within those byte lanes, the
		/// actual bytes that have WSTRB asserted can differ for each beat in the burst.
		/// </summary>
		/// <remarks>
		/// This burst type is used for repeated accesses to the same location such as when loading or emptying a FIFO.
		/// </remarks>
		Fixed = 0,

		/// <summary>
		/// Incrementing. In an incrementing burst, the address for each transfer in the burst is an increment of
		/// the address for the previous transfer. The increment value depends on the size of the transfer. For
		/// example, the address for each transfer in a burst with a size of 4 bytes is the previous address plus
		/// four.
		/// </summary>
		/// <remarks>This burst type is used for accesses to normal sequential memory.</remarks>
		Increment = 1,

		/// <summary>
		/// A wrapping burst is similar to an incrementing burst, except that the address wraps around to a lower
		/// address if an upper address limit is reached.
		/// </summary>
		/// <remarks>This burst type is used for cache line accesses.</remarks>
		Wrap = 2,

		/// <summary>
		/// Reserved for future use.
		/// </summary>
		Reserved = 3,
	}
}
