//********************************************************************************************************************************
// Filename:    NamedTypeDictionary.cs
// Owner:       Richard Dunkley
// Description: Contains a dictionary for named types.
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
	///   Represents a collection of keys and values.
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the dictionary. This type must support the <see cref="INamedType"/> interface.</typeparam>
	/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
	public class NamedTypeDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : INamedType
	{
		#region Fields

		/// <summary>
		///   Lookup table for the keys that allows them to be looked up by name.
		/// </summary>
		Dictionary<string, TKey> mLookup;

		#endregion Fields

		#region Indexers

		/// <summary>
		///   Gets or sets the value associated with the specified key's name.
		/// </summary>
		/// <param name="name">The name of the element to get or set.</param>
		/// <returns>The element with the specified name.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///   <paramref name="name"/> does not reference an item in the list.
		/// </exception>
		public TValue this[string name]
		{
			get
			{
				if (!mLookup.ContainsKey(name))
					throw new ArgumentOutOfRangeException("name does not reference an item in the dictionary.");
				return this[mLookup[name]];
			}
			set
			{
				if (!mLookup.ContainsKey(name))
					throw new ArgumentOutOfRangeException("name does not reference an item in the dictionary.");
				this[mLookup[name]] = value;
			}
		}

		#endregion Indexers

		#region Methods

		/// <summary>
		///   Initializes a new instance of the <see cref="NamedTypeDictionary{TKey, TValue}"/> class that is empty
		///   and has the default initial capacity, and uses the default equality comparer for the key type.
		/// </summary>
		public NamedTypeDictionary() : base()
		{
			mLookup = new Dictionary<string, TKey>();
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="NamedTypeDictionary{TKey, TValue}"/> class that is empty
		///   and has the specified initial capacity, and uses the default equality
		//     comparer for the key type.
		/// </summary>
		/// <param name="capacity">The number of elements that the new dictionary can initially store.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
		public NamedTypeDictionary(int capacity) : base(capacity)
		{
			mLookup = new Dictionary<string, TKey>(capacity);
		}

		/// <summary>
		///   Adds the specified key and value to the dictionary.
		/// </summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add. The value can be null for reference types.</param>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
		/// <exception cref="ArgumentException">An element with the same key or key name already exists in the dictionary.</exception>
		public new void Add(TKey key, TValue value)
		{
			if (mLookup.ContainsKey(key.Name))
				throw new ArgumentException(string.Format("An element with the same key name ({0}) already exists in the dictionary", key.Name));
			mLookup.Add(key.Name, key);
			base.Add(key, value);
		}

		/// <summary>
		///   Removes all keys and values from the dictionary.
		/// </summary>
		public new void Clear()
		{
			mLookup.Clear();
			base.Clear();
		}

		/// <summary>
		///   Determines whether the dictionary contains the specified key given it's specified name.
		/// </summary>
		/// <param name="name">Name of the key to locate in the dictionary.</param>
		/// <returns>True if the dictionary contains an element with that name; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
		public bool ContainsKey(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			return mLookup.ContainsKey(name);
		}

		/// <summary>
		///   Removes the value with the specified key from the dictionary.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <returns>
		///   True if the element is successfully found and removed; otherwise, false. This method returns
		///   false if key is not found in the dictionary.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
		public new bool Remove(TKey key)
		{
			if (key == null)
				throw new ArgumentNullException("key");
			mLookup.Remove(key.Name);
			return base.Remove(key);
		}

		/// <summary>
		///   Removes the value with the specified key's name from the dictionary.
		/// </summary>
		/// <param name="name">The name of the key of the element to remove.</param>
		/// <returns>
		///   True if the element is successfully found and removed; otherwise, false. This method returns
		///   false if key is not found in the dictionary.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
		public bool Remove(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if(mLookup.ContainsKey(name))
			{
				base.Remove(mLookup[name]);
				mLookup.Remove(name);
				return true;
			}
			return false;
		}

		/// <summary>
		///   Gets the value associated with the key's specified name.
		/// </summary>
		/// <param name="name">The name of the key of the value to get.</param>
		/// <param name="value">
		///   When this method returns, contains the value associated with the specified key,
		///   if the key is found; otherwise, the default value for the type of the value parameter.
		///   This parameter is passed uninitialized.
		/// </param>
		/// <returns>
		///   True if the dictionary contains an element with the specified key; otherwise, false.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
		public bool TryGetValue(string name, out TValue value)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if(mLookup.ContainsKey(name))
			{
				value = this[mLookup[name]];
				return true;
			}
			value = default(TValue);
			return false;
		}

		#endregion Methods
	}
}
