//********************************************************************************************************************************
// Filename:    PortInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components of a VHDL port.
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
	///   Represents a VHDL entity port.
	/// </summary>
	public class PortInfo : SimplifiedPortInfo
	{
		#region Properties

		/// <summary>
		///   Description of the port.
		/// </summary>
		public string Description { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="PortInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the port.</param>
		/// <param name="description">Description of the port.</param>
		/// <param name="direction">Direction of the port.</param>
		/// <param name="type">Type of the port.</param>
		/// <param name="defaultValue">Default value of the port. Can be null or empty.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="type"/>, or <paramref name="description"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/>, <paramref name="type"/>, or <paramref name="description"/> is an empty string.</exception>
		public PortInfo(string name, string description, PortDirection direction, string type, string defaultValue = null)
			: base(name, direction, type, defaultValue)
		{
			if (description == null)
				throw new ArgumentNullException("description");
			if (description.Length == 0)
				throw new ArgumentException("description is an empty string");

			Description = description;
		}

		/// <summary>
		///   Gets the documentation string for this port.
		/// </summary>
		/// <returns>String representing the documentation string.</returns>
		public virtual string GetDocumentationString()
		{
			return string.Format("{0} - {1}", Name, Description);
		}

		#endregion Methods
	}
}
