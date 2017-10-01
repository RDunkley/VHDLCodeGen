//********************************************************************************************************************************
// Filename:    BaseTypeInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components for a base type of VHDL component.
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
	///   Abstract base class of all the VHDL types.
	/// </summary>
	public abstract class BaseTypeInfo : IComparable<BaseTypeInfo>, INamedType
	{
		#region Properties

		/// <summary>
		///   Name of the type.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		///   Specifies the documentation remarks that will be generated with the type. Can be null or empty.
		/// </summary>
		public string Remarks { get; set; }

		/// <summary>
		///   Specifies the documentation summary that will be generated with the type.
		/// </summary>
		public string Summary { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="BaseTypeInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the type.</param>
		/// <param name="summary">Summary description of the type.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/>, or <paramref name="summary"/> is an empty string.</exception>
		public BaseTypeInfo(string name, string summary, string remarks = null)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string");
			if (summary == null)
				throw new ArgumentNullException("summary");
			if (summary.Length == 0)
				throw new ArgumentException("summary is an empty string");

			Name = name;
			Summary = summary;
			Remarks = remarks;
		}

		/// <summary>
		///   Compares a <see cref="BaseTypeInfo"/> object with this object and returns an integer that indicates their relative position in the sort order.
		/// </summary>
		/// <param name="other">Other <see cref="BaseTypeInfo"/> object to compare this object to.</param>
		/// <returns>
		///   A 32-bit signed integer that indicates the lexical relationship between the two comparands. Less than zero, this object preceeds 
		///   <paramref name="other"/>. Zero, they have the same sort order. Greater than zero, this object is after <paramref name="other"/> in the sort order.
		/// </returns>
		public int CompareTo(BaseTypeInfo other)
		{
			return string.Compare(this.Name, other.Name);
		}

		/// <summary>
		///   Gets the name of the type or derived type.
		/// </summary>
		/// <returns>String representing the name.</returns>
		public string GetTypeName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		///   Validates that the array of objects are unique and have unique names.
		/// </summary>
		/// <param name="types">Array of <see cref="BaseTypeInfo"/> objects to compare.</param>
		/// <param name="parentName">Name of the parent object.</param>
		/// <param name="parentType">Type of the parent object.</param>
		/// <exception cref="ArgumentException"><paramref name="parentName"/>, or <paramref name="parentType"/> is an empty string.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="types"/>, <paramref name="parentName"/>, or <paramref name="parentType"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">Two objects are the same or have the same name.</exception>
		public static void ValidateNoDuplicates(BaseTypeInfo[] types, string parentName, string parentType)
		{
			if (types == null)
				throw new ArgumentNullException("types");
			if (parentName == null)
				throw new ArgumentNullException("parentName");
			if (parentName.Length == 0)
				throw new ArgumentException("parentName is an empty string");
			if (parentType == null)
				throw new ArgumentNullException("parentType");
			if (parentType.Length == 0)
				throw new ArgumentException("parentType is an empty string");

			Dictionary<string, BaseTypeInfo> childNames = new Dictionary<string, BaseTypeInfo>(types.Length);
			foreach (BaseTypeInfo type in types)
			{
				if (childNames.ContainsKey(type.Name))
					BaseTypeInfo.ValidateUniqueNames(type, childNames[type.Name], parentName, parentType);
				childNames.Add(type.Name, type);
			}
		}

		/// <summary>
		///   Validates that the two objects are not equal and have unique names.
		/// </summary>
		/// <param name="info1">First <see cref="BaseTypeInfo"/> object to compare.</param>
		/// <param name="info2">Second <see cref="BaseTypeInfo"/> object to compare.</param>
		/// <param name="parentName">Name of the parent object.</param>
		/// <param name="parentType">Type of the parent object.</param>
		/// <exception cref="ArgumentException"><paramref name="parentName"/>, or <paramref name="parentType"/> is an empty string.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="info1"/>, <paramref name="info2"/>, <paramref name="parentName"/>, or <paramref name="parentType"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">The objects are the same or have the same name.</exception>
		private static void ValidateUniqueNames(BaseTypeInfo info1, BaseTypeInfo info2, string parentName, string parentType)
		{
			if (info1 == null)
				throw new ArgumentNullException("info1");
			if (info2 == null)
				throw new ArgumentNullException("info2");
			if (parentName == null)
				throw new ArgumentNullException("parentName");
			if (parentName.Length == 0)
				throw new ArgumentException("parentName is an empty string");
			if (parentType == null)
				throw new ArgumentNullException("parentType");
			if (parentType.Length == 0)
				throw new ArgumentException("parentType is an empty string");

			if (string.Compare(info1.Name, info2.Name, true) != 0)
				return;

			string name1 = info1.GetTypeName();
			if(info1 is DeclarationInfo)
			{
				DeclarationInfo info = info1 as DeclarationInfo;
				name1 = Enum.GetName(typeof(DeclarationType), info.Declaration);
			}

			string name2 = info2.GetTypeName();
			if (info2 is DeclarationInfo)
			{
				DeclarationInfo info = info2 as DeclarationInfo;
				name2 = Enum.GetName(typeof(DeclarationType), info.Declaration);
			}

			if (info1 == info2)
			{
				throw new InvalidOperationException(string.Format
				(
					"A {0} ({1}) contains duplicate {2} object children ({2}).",
					parentType,
					parentName,
					name1,
					info1.Name
				));
			}

			if (string.Compare(name1, name2, true) == 0)
			{
				throw new InvalidOperationException(string.Format
				(
					"A {0} ({1}) contains two {2} object children with the same name ({2}).",
					parentType,
					parentName,
					name1,
					info1.Name
				));
			}
			else
			{
				throw new InvalidOperationException(string.Format
				(
					"A {0} ({1}) contains a child {2} and {3} object with the same name ({4}).",
					parentType,
					parentName,
					name1,
					name2,
					info1.Name
				));
			}
		}

		/// <summary>
		///   Writes the type to a stream.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the type to.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">The current state of the type is such that it can't be written.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public abstract void Write(StreamWriter wr, int indentOffset);

		/// <summary>
		///   Writes the basic file header composed of the summary and remarks section.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the header to.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public void WriteBasicHeader(StreamWriter wr, int indentOffset)
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
		}

		/// <summary>
		///   Writes the types to a stream.
		/// </summary>
		/// <param name="regionName">Name of the region for the types.</param>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the types to.</param>
		/// <param name="types">Array of <see cref="BaseTypeInfo"/> objects to write to the stream.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <param name="parentName">Name of the parent object.</param>
		/// <param name="parentType">Type of the parent object.</param>
		/// <exception cref="ArgumentException"><paramref name="parentName"/>, or <paramref name="parentType"/> is an empty string.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/>, <paramref name="parentName"/>, or <paramref name="parentType"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">A duplicate object or name is found in the <paramref name="types"/>.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static void WriteBaseTypeInfos(string regionName, StreamWriter wr, BaseTypeInfo[] types, int indentOffset, string parentName, string parentType)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (types == null)
				throw new ArgumentNullException("types");
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
			if (types.Length == 0)
				return;

			// Validate that we don't have duplicate names.
			BaseTypeInfo.ValidateNoDuplicates(types, parentName, parentType);

			List<BaseTypeInfo> list = new List<BaseTypeInfo>();
			list.AddRange(types);
			list.Sort();

			if(!string.IsNullOrWhiteSpace(regionName))
				DocumentationHelper.WriteRegionStart(wr, regionName, indentOffset);
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Write(wr, indentOffset);
				if (i != list.Count - 1)
					DocumentationHelper.WriteLine(wr); // Leave a line between declarations.
			}
		}

		#endregion Methods
	}
}
