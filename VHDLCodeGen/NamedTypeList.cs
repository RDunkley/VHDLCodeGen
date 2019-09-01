//********************************************************************************************************************************
// Filename:    NamedTypeList.cs
// Owner:       Richard Dunkley
// Description: Contains the data components for a VHDL component instantiation.
//********************************************************************************************************************************
// Copyright © Richard Dunkley 2017
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0  Unless required by applicable
// law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
// limitations under the License.
//********************************************************************************************************************************
using System;
using System.Collections.Generic;

namespace VHDLCodeGen
{
	/// <summary>
	///   Represents a strongly typed list of objects that can be accessed by index. Provides methods to search, sort, and
	///   manipulate lists.
	/// </summary>
	/// <typeparam name="T">The type of elements in the list. Must support the <see cref="INamedType"/> interface.</typeparam>
	public class NamedTypeList<T> : List<T> where T : INamedType
	{
		#region Fields

		/// <summary>
		///   Lookup table for the elements that allows them to be looked up by name.
		/// </summary>
		Dictionary<string, T> mLookup;

		#endregion Fields

		#region Indexers

		/// <summary>
		///   Gets the element referenced by the specified name.
		/// </summary>
		/// <param name="name">The name of the element to get or set.</param>
		/// <returns>The element with the specified name.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///   <paramref name="name"/> does not reference an item in the list.
		/// </exception>
		public T this[string name]
		{
			get
			{
				if (!mLookup.ContainsKey(name))
					throw new ArgumentOutOfRangeException("name does not reference an item in the list.");
				return mLookup[name];
			}
		}

		#endregion Indexers

		#region Methods

		/// <summary>
		///   Initializes a new instance of the <see cref="NamedTypeList{T}"/> class that is empty and has
		///   the default initial capacity.
		/// </summary>
		public NamedTypeList() : base()
		{
			mLookup = new Dictionary<string, T>();
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="NamedTypeList{T}"/> class that is empty and has
		///   the specified initial capacity.
		/// </summary>
		/// <param name="capacity">The number of elements that the new list can initially store.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
		public NamedTypeList(int capacity) : base(capacity)
		{
			mLookup = new Dictionary<string, T>(capacity);
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="NamedTypeList{T}"/> class that contains elements
		///   copied from the specified collection and has sufficient capacity to accommodate the number of
		///   elements copied.
		/// </summary>
		/// <param name="collection">The collection whose elements are copied to the new list.</param>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
		/// <exception cref="ArgumentException">
		///   <paramref name="collection"/> contains multiple objects with the same name.
		/// </exception>
		public NamedTypeList(IEnumerable<T> collection) : base(collection)
		{
			mLookup = new Dictionary<string, T>();
			foreach (T info in collection)
			{
				if (mLookup.ContainsKey(info.Name))
					throw new ArgumentException("collection contained two or more objects with the same name.");
				mLookup.Add(info.Name, info);
			}
		}

		/// <summary>
		///   Adds an object to the end of the list.
		/// </summary>
		/// <param name="item">The object to be added to the end of the list. The value can be null for reference types.</param>
		public new void Add(T item)
		{
			if (mLookup.ContainsKey(item.Name))
				throw new ArgumentException(string.Format("item with that name ({0}) has already been added to the list.", item.Name));

			mLookup.Add(item.Name, item);
			base.Add(item);
		}

		/// <summary>
		///   Removes all elements from the list.
		/// </summary>
		public new void Clear()
		{
			mLookup.Clear();
			base.Clear();
		}

		/// <summary>
		///   Removes the first occurrence of a specific object from the list.
		/// </summary>
		/// <param name="item">The object to remove from the list. The value can be null for reference types.</param>
		/// <returns>True if item is successfully removed; otherwise, false. This method also returns false if item was not found in the list.</returns>
		public new bool Remove(T item)
		{
			if (mLookup.ContainsKey(item.Name))
				mLookup.Remove(item.Name);
			return base.Remove(item);
		}

		/// <summary>
		///   Removes the element at the specified index of the list.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		///		<paramref name="index"/> is less than 0 or <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
		/// </exception>
		public new void RemoveAt(int index)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException("index", index, "index is less than 0");
			if (index >= this.Count)
				throw new ArgumentOutOfRangeException("index", index, "index is equal to or greater than Count");

			if (mLookup.ContainsKey(this[index].Name))
				mLookup.Remove(this[index].Name);
			base.RemoveAt(index);
		}

		#endregion Methods
	}
}
