//********************************************************************************************************************************
// Filename:    SimplifiedGenericInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the simplified data components of a generic.
//********************************************************************************************************************************
// Copyright © Richard Dunkley 2016
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
	///   Represents a simplified VHDL generic.
	/// </summary>
	public class SimplifiedGenericInfo
	{
		#region Properties

		/// <summary>
		///   Default value of the generic. Can be null or empty.
		/// </summary>
		public string DefaultValue { get; set; }

		/// <summary>
		///   Name of the generic.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		///   Type of the generic.
		/// </summary>
		public string Type { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="SimplifiedGenericInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the generic.</param>
		/// <param name="type">Type of the generic.</param>
		/// <param name="defaultValue">Default value of the generic. Can be null or empty.</param>
		/// <exception cref="ArgumentNullException"><paramref name="type"/>, or <paramref name="name"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/>, or <paramref name="name"/> is an empty string.</exception>
		public SimplifiedGenericInfo(string name, string type, string defaultValue = null)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.Length == 0)
				throw new ArgumentException("type is an empty string");
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string");

			Name = name;
			Type = type;
			DefaultValue = defaultValue;
		}

		#endregion Methods
	}
}
