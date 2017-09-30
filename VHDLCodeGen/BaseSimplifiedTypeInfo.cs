//********************************************************************************************************************************
// Filename:    BaseSimplifiedTypeInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components of a VHDL port.
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

namespace VHDLCodeGen
{
	/// <summary>
	///   Represents a simplified VHDL type.
	/// </summary>
	public abstract class BaseSimplifiedTypeInfo : IComparable<BaseSimplifiedTypeInfo>
	{
		#region Properties

		/// <summary>
		///   Default value of the type. Can be null or empty.
		/// </summary>
		public string DefaultValue { get; set; }

		/// <summary>
		///   Name of the type.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		///   Type of the type.
		/// </summary>
		public string Type { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="BaseSimplifiedTypeInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the object.</param>
		/// <param name="type">Type of the object.</param>
		/// <param name="defaultValue">Default value of the object. Can be null or empty.</param>
		/// <exception cref="ArgumentNullException"><paramref name="type"/>, or <paramref name="name"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/>, or <paramref name="name"/> is an empty string.</exception>
		public BaseSimplifiedTypeInfo(string name, string type, string defaultValue = null)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string");
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.Length == 0)
				throw new ArgumentException("type is an empty string");

			Name = name;
			DefaultValue = defaultValue;
			Type = type;
		}

		/// <summary>
		///   Compares a <see cref="BaseSimplifiedTypeInfo"/> object with this object and returns an integer that indicates their relative position in the sort order.
		/// </summary>
		/// <param name="other">Other <see cref="BaseSimplifiedTypeInfo"/> object to compare this object to.</param>
		/// <returns>
		///   A 32-bit signed integer that indicates the lexical relationship between the two comparands. Less than zero, this object preceeds 
		///   <paramref name="other"/>. Zero, they have the same sort order. Greater than zero, this object is after <paramref name="other"/> in the sort order.
		/// </returns>
		public int CompareTo(BaseSimplifiedTypeInfo other)
		{
			return string.Compare(this.Name, other.Name);
		}

		/// <summary>
		///   Gets the declaration of the type.
		/// </summary>
		/// <returns>Declaration of the type.</returns>
		public string GetDeclaration()
		{
			string defaultValueString = string.Empty;
			if (!string.IsNullOrWhiteSpace(DefaultValue))
				defaultValueString = string.Format(" := {0}", DefaultValue);

			return GetDeclarationString(defaultValueString);
		}

		/// <summary>
		///   Gets the string for the declaration.
		/// </summary>
		/// <param name="defaultValueString">Default value string. Can be empty.</param>
		/// <returns>Complete declaration string.</returns>
		protected abstract string GetDeclarationString(string defaultValueString);

		#endregion Methods

	}
}
