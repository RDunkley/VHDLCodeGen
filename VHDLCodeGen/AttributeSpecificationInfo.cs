//********************************************************************************************************************************
// Filename:    AttributeSpecificationInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components for a VHDL attribute specification.
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
	///   Represents a VHDL attribute specification.
	/// </summary>
	public class AttributeSpecificationInfo
	{
		#region Properties

		/// <summary>
		///   The associated <see cref="AttributeDeclarationInfo"/> object containing the attribute information.
		/// </summary>
		public AttributeDeclarationInfo Declaration { get; protected set; }

		/// <summary>
		///   The item the attribute is tied to.
		/// </summary>
		public BaseTypeInfo Item { get; protected set; }

		/// <summary>
		///   Specifies the documentation remarks that will be generated with the attribute. Can be null or empty.
		/// </summary>
		public string Remarks { get; set; }

		/// <summary>
		///   Specifies the documentation summary that will be generated with the attribute.
		/// </summary>
		public string Summary { get; protected set; }

		/// <summary>
		///   The value of the attribute.
		/// </summary>
		public string Value { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="AttributeSpecificationInfo"/> object.
		/// </summary>
		/// <param name="declaration"><see cref="AttributeDeclarationInfo"/> object representing the declaration of the attribute.</param>
		/// <param name="item"><see cref="BaseTypeInfo"/> object representing the item the attribute is applied to.</param>
		/// <param name="value">Value of the attribute.</param>
		/// <param name="summary">Summary description of the attribute specification.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <remarks>Currently only <see cref="DeclarationInfo"/> and <see cref="SignalInfo"/> types are supported for <paramref name="item"/>.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="declaration"/>, <paramref name="item"/>, <paramref name="value"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/>, or <paramref name="summary"/> is an empty string or <paramref name="item"/> is not a supported type.</exception>
		public AttributeSpecificationInfo(AttributeDeclarationInfo declaration, BaseTypeInfo item, string value, string summary, string remarks = null)
		{
			if (declaration == null)
				throw new ArgumentNullException("declaration");
			if (item == null)
				throw new ArgumentNullException("item");
			if (value == null)
				throw new ArgumentNullException("value");
			if (value.Length == 0)
				throw new ArgumentException("value is an empty string");
			if (summary == null)
				throw new ArgumentNullException("summary");
			if (summary.Length == 0)
				throw new ArgumentException("summary is an empty string");

			if (!(item is DeclarationInfo) && !(item is SignalInfo))
				throw new ArgumentException("item is not a DeclarationInfo or SignalInfo type");

			Declaration = declaration;
			Item = item;
			Value = value;
			Summary = summary;
			Remarks = remarks;
		}

		#endregion Methods
	}
}
