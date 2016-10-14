//********************************************************************************************************************************
// Filename:    DeclarationInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components of a constant, type, or subtype declaration in an architectural definition of an
//              entity.
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
using System.Collections.Generic;

namespace VHDLCodeGen
{
	/// <summary>
	///   A declared type in the VHDL module.
	/// </summary>
	public class DeclarationInfo : BaseTypeInfo
	{
		#region Properties

		/// <summary>
		///   Type of the declaration or the declaration.
		/// </summary>
		public DeclarationType Declaration { get; protected set; }

		/// <summary>
		///   Default value of the declaration. Can be null or empty.
		/// </summary>
		/// 
		/// <remarks>Only valid if the <see cref="Type"/> is set to <see cref="DeclarationType.Constant"/>.</remarks>
		public string DefaultValue { get; set; }

		/// <summary>
		///   List of <see cref="DeclarationInfo"/> objects that this object is dependent upon.
		/// </summary>
		public List<DeclarationInfo> Dependency { get; private set; }

		/// <summary>
		///   Type of the declaration.
		/// </summary>
		public string Type { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="DeclarationInfo"/> object.
		/// </summary>
		/// <param name="declaration"><see cref="DeclarationType"/> of the declaration.</param>
		/// <param name="name">Name of the declaration.</param>
		/// <param name="type">Type of the declaration.</param>
		/// <param name="summary">Summary description of the declaration.</param>
		/// <param name="defaultValue">Default value of the declaration. Can be null or empty.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="type"/>, <paramref name="name"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/>, <paramref name="name"/>, or <paramref name="summary"/> is an empty string.</exception>
		/// <exception cref="InvalidOperationException"><paramref name="defaultValue"/> is specified when the <paramref name="declaration"/> is not a <see cref="DeclarationType.Constant"/>.</exception>
		public DeclarationInfo(DeclarationType declaration, string name, string type, string summary, string defaultValue = null, string remarks = null)
			: base(name, summary, remarks)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.Length == 0)
				throw new ArgumentException("type is an empty string");

			if(defaultValue != null && defaultValue.Length > 0 && declaration != DeclarationType.Constant)
			{
				throw new InvalidOperationException(string.Format
				(
					"An attempt was made to instantiate a DeclarationInfo object with a default value ({0}), for a non-constant ({1}). Only constants can have default values.",
					defaultValue,
					Enum.GetName(typeof(DeclarationType), declaration)
				));
			}

			Declaration = declaration;
			Type = type;
			DefaultValue = defaultValue;
		}

		#endregion Methods
	}
}
