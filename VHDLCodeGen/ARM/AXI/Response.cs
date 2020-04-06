//********************************************************************************************************************************
// Filename:    Response.cs
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
	///   Enumerates the various response types on an AXI bus.
	/// </summary>
	public enum Response
	{
		/// <summary>
		///   Normal access success. Indicates that a normal access has been successful. Can also indicate that an exclusive access has failed.
		/// </summary>
		OKAY = 0,

		/// <summary>
		///   Exclusive access okay. Indicates that either the read or write portion of an exclusive access has been successful.
		/// </summary>
		EXOKAY = 1,

		/// <summary>
		///   Slave error. Used when the access has reached the slave successfully, but the slave wishes to return an error condition to the originating master.
		/// </summary>
		/// <remarks>
		///   Examples of slave error conditions are: FIFO or buffer overrun or underrun condition, write access attempted to read-only location, timeout condition in the slave, or access attempted to a disabled or powered-down function.
		/// </remarks>
		SLVERR = 2,

		/// <summary>
		///   Decode error. Generated, typically by an interconnect component, to indicate that there is no slave at the transaction address.
		/// </summary>
		DECERR = 3,
	}
}
