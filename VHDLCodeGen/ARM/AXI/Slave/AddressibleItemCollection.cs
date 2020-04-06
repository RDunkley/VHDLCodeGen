//********************************************************************************************************************************
// Filename:    AddressableItemCollection.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace VHDLCodeGen.ARM.AXI.Slave
{
	/// <summary>
	///   Maintains a collection of <see cref="AddressableItemInfo"/> objects.
	/// </summary>
	public class AddressibleItemCollection : IEnumerable, IEnumerable<AddressableItemInfo>, IEqualityComparer<AddressableItemInfo>
	{
		#region Fields

		/// <summary>
		///   Lookup table to reference the <see cref="AddressableItemInfo"/> using it's designator.
		/// </summary>
		protected Dictionary<string, AddressableItemInfo> mDesignatorLookup = new Dictionary<string, AddressableItemInfo>();

		/// <summary>
		///   Tracks the readable items in the collection.
		/// </summary>
		protected List<AddressableItemInfo> mReadItems = new List<AddressableItemInfo>();

		/// <summary>
		///   Lookup table to reference all the readable <see cref="AddressableItemInfo"/> objects at a given offset.
		/// </summary>
		protected Dictionary<ulong, List<AddressableItemInfo>> mReadOffsetLookup = new Dictionary<ulong, List<AddressableItemInfo>>();

		/// <summary>
		///   Tracks the writeable items in the collection.
		/// </summary>
		protected List<AddressableItemInfo> mWriteItems = new List<AddressableItemInfo>();

		/// <summary>
		///   Lookup table to reference all the writeable <see cref="AddressableItemInfo"/> objects at a given offset.
		/// </summary>
		protected Dictionary<ulong, List<AddressableItemInfo>> mWriteOffsetLookup = new Dictionary<ulong, List<AddressableItemInfo>>();

		#endregion

		#region Properties

		/// <summary>
		///   True if the collection allows multiple items at a given offset, false if it does not.
		/// </summary>
		public bool AllowMultipleAtOffset { get; private set; }

		/// <summary>
		///   Gets the number of <see cref="AddressableItemInfo"/> objects in the collection.
		/// </summary>
		public int Count { get { return mDesignatorLookup.Count; } }

		/// <summary>
		///   Gets the number of readable <see cref="AddressableItemInfo"/> objects in the collection.
		/// </summary>
		public int ReadCount { get { return mReadItems.Count; } }

		/// <summary>
		///   Gets the number of readable <see cref="AddressableItemInfo"/> objects in the collection.
		/// </summary>
		public int WriteCount { get { return mWriteItems.Count; } }

		#endregion Properties

		#region Indexers

		/// <summary>
		///   Gets the <see cref="AddressableItemInfo"/> with the specified designator.
		/// </summary>
		/// <param name="designator">Unique designator of the <see cref="AddressableItemInfo"/>.</param>
		/// <returns><see cref="AddressableItemInfo"/> object with the <paramref name="designator"/> or null if none was found in the collection.</returns>
		public AddressableItemInfo this[string designator]
		{
			get
			{
				if (designator == null)
					return null;
				if (!mDesignatorLookup.ContainsKey(designator))
					return null;
				return mDesignatorLookup[designator];
			}
		}

		/// <summary>
		///   Gets the <see cref="AddressableItemInfo"/> at the specified offset with the specified accessibility.
		/// </summary>
		/// <param name="access">Accessibility of the item to retrieve.</param>
		/// <param name="offset">Offset of the item.</param>
		/// <returns>
		///   <see cref="AddressableItemInfo"/> object with the <paramref name="access"/> accessibility and at <paramref name="offset"/> or null
		///   if no an item could not be located.
		/// </returns>
		/// <remarks>
		///   If <see cref="AllowMultipleAtOffset"/> is true then the collection could have multiple <see cref="AddressableItemInfo"/>s at the given
		///   offset. This currently will only return the first one.
		/// </remarks>
		public AddressableItemInfo this[Access access, ulong offset]
		{
			get
			{
				if (access.HasFlag(Access.Read))
				{
					if (!mReadOffsetLookup.ContainsKey(offset))
						return null;
					return mReadOffsetLookup[offset][0];
				}
				else if(access.HasFlag(Access.Write))
				{
					if (!mWriteOffsetLookup.ContainsKey(offset))
						return null;
					return mWriteOffsetLookup[offset][0];
				}
				return null;
			}
		}

		/// <summary>
		///   Gets the numbered <see cref="AddressableItemInfo"/> at the specified offset with the specified accessibility.
		/// </summary>
		/// <param name="access">Accessibility of the item to retrieve.</param>
		/// <param name="offset">Offset of the item.</param>
		/// <param name="num">Number of the item at that offset.</param>
		/// <returns>
		///   <see cref="AddressableItemInfo"/> object with the <paramref name="access"/> accessibility and at <paramref name="offset"/> or null
		///   if no an item could not be located.
		/// </returns>
		/// <remarks>
		///   If <see cref="AllowMultipleAtOffset"/> is true then the collection could have multiple <see cref="AddressableItemInfo"/>s at the given
		///   offset. This currently will return the item specified by <paramref name="num"/>.
		/// </remarks>
		public AddressableItemInfo this[Access access, ulong offset, int num]
		{
			get
			{
				if (access.HasFlag(Access.Read))
				{
					if (!mReadOffsetLookup.ContainsKey(offset))
						return null;
					return mReadOffsetLookup[offset][num];
				}
				else if (access.HasFlag(Access.Write))
				{
					if (!mWriteOffsetLookup.ContainsKey(offset))
						return null;
					return mWriteOffsetLookup[offset][num];
				}
				return null;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="AddressibleItemCollection"/>.
		/// </summary>
		/// <param name="allowMultipleAtOffset">True if multiple items should be allowed at a given offset, false otherwise.</param>
		/// <remarks>Some items like register values can have multiple items at a single register. Memory blocks would not be able to.</remarks>
		public AddressibleItemCollection(bool allowMultipleAtOffset = false)
		{
			AllowMultipleAtOffset = allowMultipleAtOffset;
		}

		/// <summary>
		///   Add the <see cref="AddressableItemInfo"/> to the collection.
		/// </summary>
		/// <param name="item"><see cref="AddressableItemInfo"/> item to be added to the collection.</param>
		/// <exception cref="ArgumentException">
		///   <paramref name="item"/>'s accessilibity is <see cref="Access.None"/>, the collection already contains an item with that
		///   designator, or <see cref="AllowMultipleAtOffset"/> is false and an item already exists at that offset.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is a null reference.</exception>
		public void Add(AddressableItemInfo item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));
			if (item.Accessibility == Access.None)
				throw new ArgumentException("The item's accessibility is 'None'.");

			if (mDesignatorLookup.ContainsKey(item.Designator))
				throw new ArgumentException($"An item with the specified designator {item.Designator} has already been added to the collection.");

			if (!AllowMultipleAtOffset)
			{
				if (mReadOffsetLookup.ContainsKey(item.Offset))
					throw new ArgumentException($"The collection does not allow multiple items at a single offset and an item ({mReadOffsetLookup[item.Offset][0].Designator}) at the offset ({item.Offset}) already exists.");
				if (mWriteOffsetLookup.ContainsKey(item.Offset))
					throw new ArgumentException($"The collection does not allow multiple items at a single offset and an item ({mWriteOffsetLookup[item.Offset][0].Designator}) at the offset ({item.Offset}) already exists.");
			}

			if(item.Accessibility.HasFlag(Access.Read))
			{
				if (!mReadOffsetLookup.ContainsKey(item.Offset))
					mReadOffsetLookup.Add(item.Offset, new List<AddressableItemInfo>());
				mReadOffsetLookup[item.Offset].Add(item);
				mReadItems.Add(item);
			}

			if(item.Accessibility.HasFlag(Access.Write))
			{
				if (!mWriteOffsetLookup.ContainsKey(item.Offset))
					mWriteOffsetLookup.Add(item.Offset, new List<AddressableItemInfo>());
				mWriteOffsetLookup[item.Offset].Add(item);
				mWriteItems.Add(item);
			}
			
			mDesignatorLookup.Add(item.Designator, item);
		}

		/// <summary>
		///   Checks for additional overlaps that may occurr internally in the <see cref="AddressableItemInfo"/>s.
		/// </summary>
		/// <param name="access">
		///   Accessibility to check for overlaps with. Only <see cref="Access.Read"/> or <see cref="Access.Write"/> are valid, not both.
		/// </param>
		/// <returns>This class has no notion of internal overlaps so an empty string is always returned.</returns>
		protected virtual string[] CheckForAdditionalOverlaps(Access access)
		{
			// Do nothing in the base class.
			return new string[0];
		}

		/// <summary>
		///   Checks for overlaps on items within the collection.
		/// </summary>
		/// <param name="access">
		///   Accessibility to check for overlaps with. Only <see cref="Access.Read"/> or <see cref="Access.Write"/> are valid, not both.
		/// </param>
		/// <returns>Array of strings describing the found overlaps. Will be empty if no overlaps are found.</returns>
		/// <exception cref="ArgumentException"><paramref name="access"/> is <see cref="Access.None"/> or has both read and write flags set.</exception>
		public string[] CheckForOverlaps(Access access)
		{
			if (access == Access.None)
				throw new ArgumentException("Unable to check for overlaps with access set to 'None'.", nameof(access));
			if (access.HasFlag(Access.Read) && access.HasFlag(Access.Write))
				throw new ArgumentException("Unable to check for overlaps with both read and write items (only one must be specified).", nameof(access));

			Dictionary<ulong, List<AddressableItemInfo>> lookup = mReadOffsetLookup;
			string errorType = "readable";
			if (access == Access.Write)
			{
				lookup = mWriteOffsetLookup;
				errorType = "writeable";
			}

			List<Tuple<ulong, int>> offsetList = new List<Tuple<ulong, int>>(lookup.Count);
			Dictionary<int, AddressableItemInfo> listLookup = new Dictionary<int, AddressableItemInfo>(lookup.Count);
			foreach (ulong offset in lookup.Keys)
			{
				foreach (AddressableItemInfo item in lookup[offset])
				{
					listLookup.Add(offsetList.Count, item);
					offsetList.Add(new Tuple<ulong, int>(item.Offset, item.Length));
				}
			}

			List<string> errorMessages = new List<string>();
			Tuple<int, int>[] overlaps = VHDLUtility.CheckForOverlap<ulong>(offsetList.ToArray());
			if (overlaps != null)
			{
				foreach (Tuple<int, int> overlap in overlaps)
					errorMessages.Add($"The {errorType} addressable item ({GetErrorMessageInfo(listLookup[overlap.Item1])}) overlaps with another {errorType} addressable item ({GetErrorMessageInfo(listLookup[overlap.Item2])}).");
			}
			errorMessages.AddRange(CheckForAdditionalOverlaps(access));
			return errorMessages.ToArray();
		}

		/// <summary>
		///   Checks for internal and external overlaps across multiple collections.
		/// </summary>
		/// <param name="access">Accessibility to check for overlaps on.</param>
		/// <param name="collections"><see cref="AddressibleItemCollection"/>s to check for overlaps across.</param>
		/// <returns>Array of strings containing the references to overlaps found in the collections.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="collections"/> is a null reference.</exception>
		public static string[] CheckForOverlaps(Access access, params AddressibleItemCollection[] collections)
		{
			if (collections == null)
				throw new ArgumentNullException(nameof(collections));

			List<string> errorMessages = new List<string>();

			// Check for additional overlaps up front.
			foreach (AddressibleItemCollection collection in collections)
				errorMessages.AddRange(collection.CheckForAdditionalOverlaps(access));

			// Check for address-length overlaps.
			List<Tuple<ulong, int>> addressList = new List<Tuple<ulong, int>>();
			Dictionary<int, AddressableItemInfo> lookup = new Dictionary<int, AddressableItemInfo>();
			foreach (AddressibleItemCollection collection in collections)
			{
				ulong[] offsets = collection.GetOffsets(access);
				foreach (ulong offset in offsets)
				{
					AddressableItemInfo[] items = collection.GetItemsAtOffset(offset, access);

					// Find item with the maximum size.
					AddressableItemInfo item = items[0];
					for (int i = 1; i < items.Length; i++)
					{
						if (items[i].Length > item.Length)
							item = items[i];
					}
					lookup.Add(addressList.Count, item);
					addressList.Add(new Tuple<ulong, int>(offset, item.Length));
				}
			}

			Tuple<int, int>[] overlaps = VHDLUtility.CheckForOverlap<ulong>(addressList.ToArray());
			if (overlaps != null)
			{
				foreach (Tuple<int, int> overlap in overlaps)
				{
					AddressableItemInfo info1 = lookup[overlap.Item1];
					AddressableItemInfo info2 = lookup[overlap.Item2];
					errorMessages.Add($"The {info1.TypeName} ({info1.Designator}) at {info1.Offset} overlaps with a {info2.TypeName} ({info2.Designator}).");
				}
			}

			if (errorMessages.Count == 0)
				return null;
			return errorMessages.ToArray();
		}

		/// <summary>
		///   Determines whether two object instances are equal based on their designator.
		/// </summary>
		/// <param name="x">First item.</param>
		/// <param name="y">Second item.</param>
		/// <returns>True if the objects are equal, false otherwise.</returns>
		public bool Equals(AddressableItemInfo x, AddressableItemInfo y)
		{
			if (string.Compare(x.Designator, y.Designator, false) == 0)
				return true;
			return false;
		}

		/// <summary>
		///   Gets an array of desigantors contained in the collection.
		/// </summary>
		/// <returns>Array of designators. Can be empty if collection is empty.</returns>
		public string[] GetDesignators()
		{
			return new List<string>(mDesignatorLookup.Keys).ToArray();
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>Enumerator of <see cref="AddressableItemInfo"/> objects in the collection.</returns>
		public IEnumerator<AddressableItemInfo> GetEnumerator()
		{
			return mDesignatorLookup.Values.GetEnumerator();
		}

		/// <summary>
		///   Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>Enumerator of <see cref="AddressableItemInfo"/> objects in the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return mDesignatorLookup.Values.GetEnumerator();
		}

		/// <summary>
		///   Gets error message information for the specified <see cref="AddressableItemInfo"/>.
		/// </summary>
		/// <param name="item"><see cref="AddressableItemInfo"/> to et an error message identifier of.</param>
		/// <returns>String uniquely identifying the <paramref name="item"/> to the user.</returns>
		private string GetErrorMessageInfo(AddressableItemInfo item)
		{
			return $"Designator: {item.Designator}, Offset: {item.Offset}, Length: {item.Length}";
		}

		/// <summary>
		///   Gets a hash code for the object.
		/// </summary>
		/// <param name="obj"><see cref="AddressableItemInfo"/> to obtain the hash code for.</param>
		/// <returns>Hash code based on the designator of the item.</returns>
		public int GetHashCode(AddressableItemInfo obj)
		{
			return obj.Designator.GetHashCode();
		}

		/// <summary>
		///   Gets a string containing the listing off the specified values.
		/// </summary>
		/// <param name="values"><see cref="AddressableItemInfo"/> array containing the values to list.</param>
		/// <returns>String containing a list of the values or an empty string if <paramref name="values"/> is an empty array.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="values"/> is a null reference.</exception>
		public static string GetItemList(AddressableItemInfo[] values)
		{
			if (values == null)
				throw new ArgumentNullException(nameof(values));

			if (values.Length == 0)
				return string.Empty;

			StringBuilder sb = new StringBuilder(values[0].Designator);
			for (int i = 1; i < values.Length; i++)
				sb.Append($", {values[i].Designator}");
			return sb.ToString();
		}

		/// <summary>
		///   Gets all the <see cref="AddressableItemInfo"/> objects in the collection with the specified accessibility.
		/// </summary>
		/// <param name="access">Accessibilty of the objects to retrieve.</param>
		/// <returns>Array of <see cref="AddressableItemInfo"/> objects.</returns>
		public AddressableItemInfo[] GetItems(Access access)
		{
			List<AddressableItemInfo> itemList = new List<AddressableItemInfo>();
			if (access.HasFlag(Access.Write))
				itemList.AddRange(mWriteItems);
			if (access.HasFlag(Access.Read))
				itemList.AddRange(mReadItems);
			return itemList.ToArray();
		}

		/// <summary>
		///   Gets all the items at the specified offset and with the specified accessibility.
		/// </summary>
		/// <param name="offset">Offset in the register space.</param>
		/// <param name="access">Accessibilty of the items.</param>
		/// <returns>Array of <see cref="AddressableItemInfo"/> objects or an empty array if no objects were found.</returns>
		public AddressableItemInfo[] GetItemsAtOffset(ulong offset, Access access)
		{
			List<AddressableItemInfo> itemList = new List<AddressableItemInfo>();
			if (access.HasFlag(Access.Read) && mReadOffsetLookup.ContainsKey(offset))
				itemList.AddRange(mReadOffsetLookup[offset]);
			if (access.HasFlag(Access.Write) && mWriteOffsetLookup.ContainsKey(offset))
				itemList.AddRange(mWriteOffsetLookup[offset]);
			return itemList.ToArray();
		}

		/// <summary>
		///   Gets all the offsets in the accessible register space.
		/// </summary>
		/// <param name="access">Accessibilty to return the offsets for.</param>
		/// <returns>Array of offsets. Can be empty if no offsets are found or <paramref name="access"/> is <see cref="Access.None"/>.</returns>
		/// <remarks>
		///   May return duplicate offsets if both <see cref="Access.Read"/> and <see cref="Access.Write"/> are specified for <paramref name="access"/>.
		/// </remarks>
		public ulong[] GetOffsets(Access access)
		{
			List<ulong> offsetList = new List<ulong>();
			if (access.HasFlag(Access.Write))
				offsetList.AddRange(mWriteOffsetLookup.Keys);
			if (access.HasFlag(Access.Read))
				offsetList.AddRange(mReadOffsetLookup.Keys);
			return offsetList.ToArray();
		}

		/// <summary>
		///   Returns the simplified offset/length tuples associated with the register space with the specified accessibility.
		/// </summary>
		/// <param name="access">Accessibility of the returned blocks.</param>
		/// <returns>
		///   Array of tuples representing the offset and length of registers in the register space that have associated items.
		///   Registers next to each other are combined into a single offset/length pair to simplify the mapping. This is helpful
		///   when the caller needs to determine what registers are being used in a simplified block form.
		/// </returns>
		public Tuple<ulong, int>[] GetSimplifiedBlocks(Access access)
		{
			AddressableItemInfo[] items = GetItems(access);
			List<Tuple<ulong, int>> list = new List<Tuple<ulong, int>>();
			foreach (AddressableItemInfo item in items)
				list.Add(new Tuple<ulong, int>(item.Offset, item.Length));
			return VHDLUtility.SimplifyMemoryMap(list.ToArray());
		}

		/// <summary>
		///   Returns the simplified offset/length tuples associated with the register space with the specified accessibility.
		/// </summary>
		/// <param name="access">Accessibility of the returned blocks.</param>
		/// <param name="collections"><see cref="AddressibleItemCollection"/>s to pull the items from.</param>
		/// <returns>
		///   Array of tuples representing the offset and length of registers in the register space that have associated items.
		///   Registers next to each other are combined into a single offset/length pair to simplify the mapping. This is helpful
		///   when the caller needs to determine what registers are being used in a simplified block form.
		/// </returns>
		public static Tuple<ulong, int>[] GetSimplifiedBlocks(Access access, params AddressibleItemCollection[] collections)
		{
			List<Tuple<ulong, int>> list = new List<Tuple<ulong, int>>();
			foreach(AddressibleItemCollection col in collections)
			{
				AddressableItemInfo[] items = col.GetItems(access);
				foreach (AddressableItemInfo item in items)
					list.Add(new Tuple<ulong, int>(item.Offset, item.Length));
			}
			return VHDLUtility.SimplifyMemoryMap(list.ToArray());
		}

		/// <summary>
		///   Gets all the unique register space offsets from the collections.
		/// </summary>
		/// <param name="access">Accessibilty to return the offsets for.</param>
		/// <param name="collections">Collection of <see cref="AddressibleItemCollection"/> to query for offsets.</param>
		/// <returns>Array of offsets. Can be empty if no offsets are found or <paramref name="access"/> is <see cref="Access.None"/>.</returns>
		public static ulong[] GetUniqueOffsets(Access access, params AddressibleItemCollection[] collections)
		{
			List<ulong> offsetList = new List<ulong>();
			foreach (AddressibleItemCollection col in collections)
			{
				ulong[] offsets = col.GetOffsets(access);
				foreach (ulong offset in offsets)
				{
					if (!offsetList.Contains(offset))
						offsetList.Add(offset);
				}
			}
			offsetList.Sort();
			return offsetList.ToArray();
		}

		#endregion
	}
}
