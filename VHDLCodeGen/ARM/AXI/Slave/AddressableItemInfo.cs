//********************************************************************************************************************************
// Filename:    AddressableItemInfo.cs
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
	///   Represents an addressable item in an AXI register map.
	/// </summary>
	public abstract class AddressableItemInfo
	{
		#region Properties

		/// <summary>
		///   Gets the accessibility of the item.
		/// </summary>
		public Access Accessibility { get; private set; }

		/// <summary>
		///   Gets or sets a description associated with the item. Used for code comments.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		///   Gets the unique designator associated with the item.
		/// </summary>
		/// <remarks>This item is unique across the register space of the AXI interface.</remarks>
		public string Designator { get; private set; }

		/// <summary>
		///   Gets or sets the number of bytes the item consumes in the register space. Must be a multiple of the register byte width.
		/// </summary>
		public int Length { get; private set; }

		/// <summary>
		///   Gets the user readable name associated with the item. Used for code comments and error messages.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets or sets the offset into the register space assocaited with the item.
		/// </summary>
		/// <remarks>Offset is the byte addressable address and must fall on a register boundary.</remarks>
		public ulong Offset { get; private set; }

		/// <summary>
		///   Gets the human readable type of the addressible item.
		/// </summary>
		public string TypeName { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		///   Instantiates a new addressable item object.
		/// </summary>
		/// <param name="typeName">Human readable name of the instantiated type.</param>
		/// <param name="designator">Unique designator of this item.</param>
		/// <param name="name">Human readable name of this item.</param>
		/// <param name="offset">Offset of this item in the register space. Must be byte addressable offset that falls on a register width boundary.</param>
		/// <param name="length">Length of the item in the register space. Must be a mutliple of the register byte width.</param>
		/// <param name="accessibility">Accessibility of the item in the register space.</param>
		/// <exception cref="ArgumentException"><paramref name="accessibility"/> is unrecognized, or the length is less than 4.</exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="typeName"/>, <paramref name="designator"/>, or <paramref name="name"/> is a null reference.
		/// </exception>
		public AddressableItemInfo(string typeName, string designator, string name, ulong offset, int length, Access accessibility)
		{
			if (string.IsNullOrEmpty(typeName))
				throw new ArgumentNullException(nameof(typeName));
			if (string.IsNullOrEmpty(designator))
				throw new ArgumentNullException(nameof(designator));
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));
			if (length < 4)
				throw new ArgumentException("The length is less than 4.", nameof(length));

			TypeName = typeName;
			Accessibility = accessibility;
			Designator = designator;
			Name = name;
			Offset = offset;
			Length = length;
		}

		#endregion
	}
}
