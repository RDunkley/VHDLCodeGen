//********************************************************************************************************************************
// Filename:    PartialRegisterInfo.cs
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
using System.Collections.ObjectModel;
using System.Text;

namespace VHDLCodeGen.ARM.AXI.Slave
{
	/// <summary>
	///   Represents part of a register value in the addressable register space.
	/// </summary>
	/// <remarks>
	///   The register value can represent an entire register or a smaller portion of a single register. Size cannot exceed the single register width.
	/// </remarks>
	public class PartialRegisterInfo : AddressableItemInfo
	{
		#region Properties

		/// <summary>
		///   Gets the width of this register's value in bits.
		/// </summary>
		/// <remarks>Equal to <see cref="FullRegisterWidth"/> when the value is a full register.</remarks>
		public int BitWidth { get; private set; }

		/// <summary>
		///   Gets the byte lane mapping of the register value in the register.
		/// </summary>
		public ReadOnlyDictionary<int, Tuple<int, int>> ByteLaneMapping { get; private set; }

		/// <summary>
		///   Gets the ending bit of the register.
		/// </summary>
		public int EndBit { get; private set; }

		/// <summary>
		///   Gets the width of the containing register in bits.
		/// </summary>
		public int FullRegisterWidth { get; private set; }

		/// <summary>
		///   Gets the starting bit of the register.
		/// </summary>
		public int StartBit { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="PartialRegisterInfo"/> object.
		/// </summary>
		/// <param name="typeName">Name of the type of partial register. Used internally for error messages.</param>
		/// <param name="designator">Unique designator of the register in the register space.</param>
		/// <param name="offset">Offset of the register in the register space. Must fall on a register boundary.</param>
		/// <param name="accessibility">Accessibility of the register in the register space.</param>
		/// <param name="start">Index of the starting bit in the register.</param>
		/// <param name="end">Index of the ending bit in the register.</param>
		/// <param name="fullRegisterWidth">Full width of the register (in bits).</param>
		/// <param name="name">Human readable name of the register.</param>
		/// <exception cref="ArgumentException">
		///   <paramref name="accessibility"/> is unrecognized.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="designator"/> is a null reference.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///   <paramref name="fullRegisterWidth"/> is not equal to 32 or 64, <paramref name="start"/> or <paramref name="end"/> are less than 0 or
		///   greater than <paramref name="fullRegisterWidth"/> - 1.
		/// </exception>
		public PartialRegisterInfo(string typeName, string designator, ulong offset, Access accessibility, int start, int end, int fullRegisterWidth, string name = null)
			: base(typeName, designator, name == null ? designator : name, offset, fullRegisterWidth/8, accessibility)
		{
			if (fullRegisterWidth != 32 && fullRegisterWidth != 64)
				throw new ArgumentOutOfRangeException(nameof(fullRegisterWidth), $"The full register width specified ({fullRegisterWidth}) is not 32 or 64. Only 32-bit and 64-bit registers are allowed.");
			if (start < 0 || start > (fullRegisterWidth - 1))
				throw new ArgumentOutOfRangeException(nameof(start), $"The starting index specified ({start}) is less than 0 or greater than {fullRegisterWidth - 1}.");
			if (end < 0 || end > (fullRegisterWidth - 1))
				throw new ArgumentOutOfRangeException(nameof(end), $"The ending index specified ({end}) is less than 0 or greater than {fullRegisterWidth - 1}.");

			if (start < end)
			{
				StartBit = start;
				EndBit = end;
			}
			else
			{
				StartBit = end;
				EndBit = start;
			}

			FullRegisterWidth = fullRegisterWidth;
			BitWidth = EndBit - StartBit + 1;
			ByteLaneMapping = VHDLUtility.GetByteLaneMapping(StartBit, EndBit, fullRegisterWidth);
		}

		#endregion
	}
}
