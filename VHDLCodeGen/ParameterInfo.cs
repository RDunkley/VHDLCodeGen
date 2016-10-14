//********************************************************************************************************************************
// Filename:    ParameterInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to describe a simplified parameter in a VHDL function.
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
	///   Represents the information of a VHDL function parameter.
	/// </summary>
	public class ParameterInfo
	{
		#region Properties

		/// <summary>
		///   Description of the parameter.
		/// </summary>
		public string Description { get; protected set; }

		/// <summary>
		///   Name of the parameter.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		///   Type of the parameter (Ex: int, string, etc.).
		/// </summary>
		public string Type { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ParameterInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the parameter.</param>
		/// <param name="type">Type of the parameter.</param>
		/// <param name="description">Description of the parameter.</param>
		/// <exception cref="ArgumentNullException"><paramref name="type"/>, <paramref name="name"/>, or <paramref name="description"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/>, <paramref name="name"/>, or <paramref name="description"/> is an empty string.</exception>
		public ParameterInfo(string name, string type, string description)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.Length == 0)
				throw new ArgumentException("type is an empty string");
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string");
			if (description == null)
				throw new ArgumentNullException("description");
			if (description.Length == 0)
				throw new ArgumentException("description is an empty string");

			Name = name;
			Type = type;
			Description = description;
		}

		#endregion Methods
	}
}
