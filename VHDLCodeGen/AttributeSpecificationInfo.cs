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
using System.Collections.Generic;
using System.IO;

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

		/// <summary>
		///   Writes the attribute specification to a stream.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the attribute specification to.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			if (indentOffset < 0)
				indentOffset = 0;

			Dictionary<string, string[]> lookup = new Dictionary<string, string[]>(2);
			lookup.Add("Summary", new string[] { Summary });
			if (!string.IsNullOrWhiteSpace(Remarks))
				lookup.Add("Remarks", new string[] { Remarks });

			DocumentationHelper.WriteFlowerLine(wr, indentOffset);
			DocumentationHelper.WriteGeneralDocumentationElements(wr, lookup, indentOffset);
			DocumentationHelper.WriteFlowerLine(wr, indentOffset);
			DocumentationHelper.WriteLine(wr, string.Format("attribute {0} of {1} : {2} is {3};", Declaration.Name, Item.Name, GetItemString(), Value), indentOffset);
		}

		/// <summary>
		///   Returns the string corresponding to the <see cref="Item"/> type.
		/// </summary>
		/// <returns>String representing the item type.</returns>
		private string GetItemString()
		{
			if(Item is DeclarationInfo)
			{
				DeclarationInfo info = Item as DeclarationInfo;
				return Enum.GetName(typeof(DeclarationType), info.Declaration);
			}

			if(Item is SignalInfo)
				return "signal";

			throw new NotImplementedException("The Item type was not a recognized type.");
		}

		/// <summary>
		///   Writes the attributes to a stream.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the types to.</param>
		/// <param name="attributes">Array of <see cref="AttributeSpecificationInfo"/> objects to write to the stream.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <param name="parentName">Name of the parent object.</param>
		/// <param name="parentType">Type of the parent object.</param>
		/// <exception cref="ArgumentException"><paramref name="parentName"/>, or <paramref name="parentType"/> is an empty string.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/>, <paramref name="parentName"/>, or <paramref name="parentType"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">A duplicate object or name is found in the <paramref name="attributes"/>.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static void WriteAttributes(StreamWriter wr, AttributeSpecificationInfo[] attributes, int indentOffset, string parentName, string parentType)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (attributes == null)
				throw new ArgumentNullException("attributes");
			if (parentName == null)
				throw new ArgumentNullException("parentName");
			if (parentName.Length == 0)
				throw new ArgumentException("parentName is an empty string");
			if (parentType == null)
				throw new ArgumentNullException("parentType");
			if (parentType.Length == 0)
				throw new ArgumentException("parentType is an empty string");

			if (indentOffset < 0)
				indentOffset = 0;

			// Write nothing if array is empty.
			if (attributes.Length == 0)
				return;
			// TODO: should validate that there aren't duplicate declaration/signal combination. - RD

			Dictionary<AttributeDeclarationInfo, List<AttributeSpecificationInfo>> lookup = new Dictionary<AttributeDeclarationInfo, List<AttributeSpecificationInfo>>();
			foreach (AttributeSpecificationInfo info in attributes)
			{
				if (!lookup.ContainsKey(info.Declaration))
					lookup.Add(info.Declaration, new List<AttributeSpecificationInfo>());
				lookup[info.Declaration].Add(info);
			}

			// Validate that we don't have duplicate names in the declarations.
			List<AttributeDeclarationInfo> list = new List<AttributeDeclarationInfo>(lookup.Keys);
			BaseTypeInfo.ValidateNoDuplicates(list.ToArray(), parentName, parentType);
			list.Sort();

			DocumentationHelper.WriteRegionStart(wr, "Attributes", indentOffset);
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Write(wr, indentOffset);

				DocumentationHelper.WriteLine(wr);
				for(int j = 0; j < lookup[list[i]].Count; j++)
				{
					lookup[list[i]][j].Write(wr, indentOffset);
					if (j != lookup[list[i]].Count - 1)
						DocumentationHelper.WriteLine(wr); // Leave a line between specifications.
				}

				if (i != list.Count - 1)
					DocumentationHelper.WriteLine(wr); // Leave a line between declarations.
			}
		}

		#endregion Methods
	}
}
