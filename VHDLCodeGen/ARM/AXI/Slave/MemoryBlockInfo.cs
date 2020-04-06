//********************************************************************************************************************************
// Filename:    MemoryBlockInfo.cs
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
	///   Represents a memory block in the addressable register space.
	/// </summary>
	/// <remarks>
	///   A memory block is one or more registers that are tied together to represent an addressable block. It is also
	///   assumed that the access time for the block may vary or the communication to the block is more complicated than
	///   a standard register set. This object can be used to describe block rams, ROMs, external busses, etc.
	/// </remarks>
	public class MemoryBlockInfo : AddressableItemInfo
	{
		#region Properties

		/// <summary>
		///   Gets or sets the name of the register that will be read at the end of a read cycle.
		/// </summary>
		/// <remarks>A default name is provided, but can be changed as required.</remarks>
		public string MemoryOutputRegisterName { get; set; }

		/// <summary>
		///   Gets or sets the name of the signal asserted at the beginning of the block's read cycle.
		/// </summary>
		/// <remarks>A default name is provided, but can be changed as required.</remarks>
		public string ReadNotificationSignalName { get; set; }

		/// <summary>
		///   Gets or sets the name of the state used while waiting for reads of the memory block.
		/// </summary>
		/// <remarks>A default name is provided, but can be changed as required.</remarks>
		public string StateName { get; set; }

		/// <summary>
		///   Gets the type of memory block.
		/// </summary>
		public MemoryType Type { get; private set; }

		/// <summary>
		///   Gets or sets the name of the signal asserted at the beginning of the block's write cycle.
		/// </summary>
		/// <remarks>A default name is provided, but can be changed as required.</remarks>
		public string WriteNotificationSignalName { get; set; }

		#endregion

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="MemoryBlockInfo"/> object.
		/// </summary>
		/// <param name="designator">Designator of the block.</param>
		/// <param name="type"><see cref="MemoryType"/> of the block.</param>
		/// <param name="offset">Offset of the block in the register space. Must fall on a register boundary.</param>
		/// <param name="length">Length of the block in bytes. Must be a multiple of the register width.</param>
		/// <param name="access">Accessibility of the block in the register space.</param>
		/// <param name="name">Human readable name of the block.</param>
		/// <exception cref="ArgumentException">
		///   <paramref name="access"/> is unrecognized, the length is less than 4, or the <paramref name="type"/>
		///   is <see cref="MemoryType.ROM"/> but the accessibility included <see cref="Access.Write"/>.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="designator"/> is a null reference.
		/// </exception>
		public MemoryBlockInfo(string designator, MemoryType type, ulong offset, int length, Access access, string name = null)
			: base("memory block", designator, name == null ? designator : name, offset, length, access)
		{
			if (type == MemoryType.ROM && access.HasFlag(Access.Write))
				throw new ArgumentException("An attempt was made to add a 'ROM' memory block with writeable access.", nameof(type));

			Type = type;
			StateName = GenerateMemoryStateName(designator);
			MemoryOutputRegisterName = string.Format("{0}_reg", designator.ToLower());
			ReadNotificationSignalName = string.Format("{0}_read", designator.ToLower());
			WriteNotificationSignalName = string.Format("{0}_write", designator.ToLower());
		}

		/// <summary>
		///   Generates the memory state name.
		/// </summary>
		/// <param name="designator">Designator of the memory.</param>
		/// <returns>The generated name.</returns>
		private string GenerateMemoryStateName(string designator)
		{
			StringBuilder sb = new StringBuilder(designator[0].ToString().ToUpper());
			bool capNext = false;
			for (int i = 1; i < designator.Length; i++)
			{
				if (designator[i] == '_')
				{
					capNext = true;
				}
				else
				{
					if (capNext)
						sb.Append(char.ToUpper(designator[i]));
					else
						sb.Append(designator[i]);
					capNext = false;
				}
			}
			return string.Format("sWait{0}", sb.ToString());
		}

		#endregion
	}
}
