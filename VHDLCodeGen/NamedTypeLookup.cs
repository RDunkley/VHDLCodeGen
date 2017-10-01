//********************************************************************************************************************************
// Filename:    NamedTypeLookup.cs
// Owner:       Richard Dunkley
// Description: Contains a lookup table for named types.
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
using System.Collections;
using System.Collections.Generic;

namespace VHDLCodeGen
{
	/// <summary>
	///   Represents a lookup table for <see cref="INamedType"/> keys and nullable values.
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the lookup table. This type must support the <see cref="INamedType"/> interface.</typeparam>
	/// <typeparam name="TValue">The type of the values in the lookup table. Must be a nullable type.</typeparam>
	public class NamedTypeLookup<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable where TKey : INamedType
	{
		#region Fields

		/// <summary>
		///   Lookup table for the keys that allows them to be looked up by name.
		/// </summary>
		Dictionary<string, TKey> mLookup;

		/// <summary>
		///   Lookup table for the values that allows them to be looked up by key.
		/// </summary>
		Dictionary<TKey, TValue> mValues;

		#endregion Fields

		#region Indexers

		/// <summary>
		///   Gets or sets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the value to get or set.</param>
		/// <returns>
		///   The value associated with the specified key. If the specified key is not found, a get operation
		///   throws a <see cref="KeyNotFoundException"/>, and a set operation creates a new element with the
		///   specified key.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
		/// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
		public TValue this[TKey key]
		{
			get
			{
				return mValues[key];
			}
			set
			{
				mValues[key] = value;
			}
		}

		/// <summary>
		///   Gets or sets the value associated with the specified key's name.
		/// </summary>
		/// <param name="name">The name of the element to get or set.</param>
		/// <returns>The element with the specified name.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///   <paramref name="name"/> does not reference an item in the table.
		/// </exception>
		public TValue this[string name]
		{
			get
			{
				if (!mLookup.ContainsKey(name))
					throw new ArgumentOutOfRangeException("name does not reference an item in the table.");
				return mValues[mLookup[name]];
			}
			set
			{
				if (!mLookup.ContainsKey(name))
					throw new ArgumentOutOfRangeException("name does not reference an item in the table.");
				mValues[mLookup[name]] = value;
			}
		}

		#endregion Indexers

		#region Properties

		/// <summary>
		///   Gets the number of key/value pairs contained in the lookup table.
		/// </summary>
		public int Count
		{
			get
			{
				return mValues.Count;
			}
		}

		/// <summary>
		///    Gets a collection containing the keys in the table.
		/// </summary>
		public IEnumerable<TKey> Keys => ((IReadOnlyDictionary<TKey, TValue>)mValues).Keys;

		/// <summary>
		///   Gets a collection containing the values in the table.
		/// </summary>
		public IEnumerable<TValue> Values => ((IReadOnlyDictionary<TKey, TValue>)mValues).Values;

		#endregion Properties

		#region Methods

		/// <summary>
		///   Initializes a new instance of <see cref="NamedTypeDictionary{TKey, TValue}"/>.
		/// </summary>
		/// <param name="list"><see cref="NamedTypeList{T}"/> containing the keys for the lookup table.</param>
		/// <remarks>Default values for the value type will populate the lookup table initially.</remarks>
		public NamedTypeLookup(NamedTypeList<TKey> list)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			if (default(TValue) != null)
				throw new InvalidOperationException("An attempt was made to create the lookup table with a non-nullable type as the value.");

			mLookup = new Dictionary<string, TKey>(list.Count);
			mValues = new Dictionary<TKey, TValue>(list.Count);

			foreach(TKey item in list)
			{
				mValues.Add(item, default(TValue));
				mLookup.Add(item.Name, item);
			}
		}


		/// <summary>
		///   Determines whether the lookup table contains the specified key given it's specified name.
		/// </summary>
		/// <param name="name">Name of the key to locate in the table.</param>
		/// <returns>True if the table contains an element with that name; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
		public bool ContainsKey(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			return mLookup.ContainsKey(name);
		}

		/// <summary>
		///   Determines whether the lookup table contains the specified key.
		/// </summary>
		/// <param name="key">The key to locate in the table.</param>
		/// <returns>
		///   True if the table contains an element with the specified key; otherwise, false.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
		public bool ContainsKey(TKey key)
		{
			return mValues.ContainsKey(key);
		}

		/// <summary>
		///   Determines whether the table contains a specific value.
		/// </summary>
		/// <param name="value">
		///   The value to locate in the lookup table. The value can be null for reference types.
		/// </param>
		/// <returns>
		///   True if the lookup table contains an element with the specified value; otherwise, false.
		/// </returns>
		public bool ContainsValue(TValue value)
		{
			return mValues.ContainsValue(value);
		}

		/// <summary>
		///   Returns an enumerator that iterates through the table.
		/// </summary>
		/// <returns>An Enumerator structure for the lookup table.</returns>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return ((IReadOnlyDictionary<TKey, TValue>)mValues).GetEnumerator();
		}

		/// <summary>
		///   Returns an enumerator that iterates through the table.
		/// </summary>
		/// <returns>An Enumerator structure for the lookup table.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IReadOnlyDictionary<TKey, TValue>)mValues).GetEnumerator();
		}

		/// <summary>
		///   Gets the value associated with the key's specified name.
		/// </summary>
		/// <param name="name">The name of the key of the value to get.</param>
		/// <param name="value">
		///   When this method returns, contains the value associated with the specified key's name, if the key is found;
		///   otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.
		/// </param>
		/// <returns>
		///   True if the lookup table contains an element with the specified key; otherwise, false.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="name"/> does not reference an item in the table.</exception>
		public bool TryGetValue(string name, out TValue value)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (!mLookup.ContainsKey(name))
				throw new ArgumentOutOfRangeException("name does not reference an item in the table.");

			return ((IReadOnlyDictionary<TKey, TValue>)mValues).TryGetValue(mLookup[name], out value);
		}

		/// <summary>
		///   Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the value to get.</param>
		/// <param name="value">
		///   When this method returns, contains the value associated with the specified key, if the key is found;
		///   otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.
		/// </param>
		/// <returns>
		///   True if the lookup table contains an element with the specified key; otherwise, false.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
		public bool TryGetValue(TKey key, out TValue value)
		{
			return ((IReadOnlyDictionary<TKey, TValue>)mValues).TryGetValue(key, out value);
		}

		#endregion Methods
	}
}
