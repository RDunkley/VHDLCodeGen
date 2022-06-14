//********************************************************************************************************************************
// Filename:    PartialRegisterInfoCollection.cs
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
	///   Maintains a collection of <see cref="PartialRegisterInfo"/> objects.
	/// </summary>
	public class PartialRegisterInfoCollection : AddressibleItemCollection
	{
		#region Properties

		/// <summary>
		///   Gets the register width (int bits) allowed in the collection.
		/// </summary>
		public int RegisterWidth { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		///   Instantiates an new <see cref="PartialRegisterInfoCollection"/>.
		/// </summary>
		/// <param name="width">Width of the registers (in bits) allowed in the collection. A register space can only be either 32-bit or 64-bit.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="width"/> is not equal to 32 or 64.</exception>
		public PartialRegisterInfoCollection(int width)
			: base(true)
		{
			if (width != 32 && width != 64)
				throw new ArgumentOutOfRangeException(nameof(width), $"The full register width specified ({width}) is not 32 or 64. Only 32-bit and 64-bit register collections are allowed.");
			RegisterWidth = width;
		}

		/// <summary>
		///   Adds a register value to the collection.
		/// </summary>
		/// <param name="item"><see cref="PartialRegisterInfo"/> to be added to the collection.</param>
		/// <exception cref="ArgumentException">
		///   <paramref name="item"/>'s accessilibity is <see cref="Access.None"/>, the collection already contains an item with that
		///   designator, or the item's register width is not equal to that of this collection.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is a null reference.</exception>
		public void Add(PartialRegisterInfo item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));
			if (item.FullRegisterWidth != RegisterWidth)
				throw new ArgumentException($"The register ({item.Name}) value's width ({item.FullRegisterWidth}) is not equal to the collection's allowed width ({RegisterWidth}).", nameof(item));
			base.Add(item);
		}

		/// <summary>
		///   Checks for additional overlaps in the individual registers.
		/// </summary>
		/// <param name="access">
		///   Accessibility to check for overlaps with. Only <see cref="Access.Read"/> or <see cref="Access.Write"/> are valid, not both.
		/// </param>
		/// <returns>This method will call <see cref="CheckForBitOverlaps(Access)"/> to check for additional overlaps.</returns>
		/// <exception cref="ArgumentException"><paramref name="access"/> has both <see cref="Access.Read"/> and <see cref="Access.Write"/> specified.</exception>
		protected override string[] CheckForAdditionalOverlaps(Access access)
		{
			return CheckForBitOverlaps(access);
		}

		/// <summary>
		///   Checks of register values in the same register that overlap with start and end indexes.
		/// </summary>
		/// <param name="access">
		///   Accessibility to check for overlaps with. Only <see cref="Access.Read"/> or <see cref="Access.Write"/> are valid, not both.
		/// </param>
		/// <returns>Array of strings describing the overlapping values.</returns>
		/// <exception cref="ArgumentException"><paramref name="access"/> has both <see cref="Access.Read"/> and <see cref="Access.Write"/> specified.</exception>
		public string[] CheckForBitOverlaps(Access access)
		{
			if (access == Access.None)
				return new string[0];
			if (access.HasFlag(Access.Read) && access.HasFlag(Access.Write))
				throw new ArgumentException("Unable to check for bit overlaps with both read and write items (only one must be specified).", nameof(access));

			Dictionary<ulong, List<AddressableItemInfo>> lookup = mReadOffsetLookup;
			string errorType = "readable";
			if (access == Access.Write)
			{
				lookup = mWriteOffsetLookup;
				errorType = "writeable";
			}

			List<string> errorMessages = new List<string>();
			foreach (ulong offset in lookup.Keys)
			{
				List<Tuple<int, int>> bitList = new List<Tuple<int, int>>(lookup[offset].Count);
				Dictionary<int, PartialRegisterInfo> regLookup = new Dictionary<int, PartialRegisterInfo>();
				foreach (PartialRegisterInfo info in lookup[offset])
				{
					regLookup.Add(bitList.Count, info);
					bitList.Add(new Tuple<int, int>(info.StartBit, info.EndBit - info.StartBit + 1));
				}

				// Check for overlaps.
				Tuple<int, int>[] regOverlaps = VHDLUtility.CheckForOverlap<int>(bitList.ToArray());
				if (regOverlaps != null)
				{
					foreach (Tuple<int, int> overlap in regOverlaps)
						errorMessages.Add($"A {errorType} register value ({GetBitErrorMessageInfo(regLookup[overlap.Item1])} overlaps with another {errorType} register value ({GetBitErrorMessageInfo(regLookup[overlap.Item2])}) in the same register (Offset: {offset}).");
				}
			}

			return errorMessages.ToArray();
		}

		/// <summary>
		///   Gets a bit error description for the specified register value.
		/// </summary>
		/// <param name="item"><see cref="PartialRegisterInfo"/> object to get the description of.</param>
		/// <returns>Unique description of the <see cref="PartialRegisterInfo"/> for use in error messages.</returns>
		private string GetBitErrorMessageInfo(PartialRegisterInfo item)
		{
			return $"Designator: {item.Designator}, Start bit: {item.StartBit}, End bit: {item.EndBit}";
		}

		/// <summary>
		///   Gets all the register values in the collection that have a portion of their value in the specified offset and byte lane.
		/// </summary>
		/// <param name="lane">
		///   Byte lane index of the requested lane. Lane 0 pertains to data bus bits (7 downto 0) according to AXI specifications.
		/// </param>
		/// <param name="offset">Offset into the register space.</param>
		/// <returns>
		///   Array of tuples with the register value item, the starting bit index, and the ending bit index in the register
		///   (in that byte lane). Can be empty if no writeable register values are at that offset.
		/// </returns>
		/// <remarks>This makes it easy to generate byte lane logic by pulling all the pertaining register values.</remarks>
		public Tuple<PartialRegisterInfo, int, int>[] GetWriteRegistersInByteLane(int lane, ulong offset)
		{
			if (!mWriteOffsetLookup.ContainsKey(offset))
				return new Tuple<PartialRegisterInfo, int, int>[0];

			List<Tuple<PartialRegisterInfo, int, int>> list = new List<Tuple<PartialRegisterInfo, int, int>>();
			foreach (PartialRegisterInfo info in mWriteOffsetLookup[offset])
			{
				if (info.ByteLaneMapping.ContainsKey(lane))
					list.Add(new Tuple<PartialRegisterInfo, int, int>(info, info.ByteLaneMapping[lane].Item1, info.ByteLaneMapping[lane].Item2));
			}
			return list.ToArray();
		}

		#endregion
	}
}
