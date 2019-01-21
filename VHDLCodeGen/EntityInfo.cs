//********************************************************************************************************************************
// Filename:    EntityInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified VHDL Entity.
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
using System.Text;

namespace VHDLCodeGen
{
	/// <summary>
	///   Represents a VHDL entity.
	/// </summary>
	public class EntityInfo : BaseTypeInfo
	{
		#region Properties

		/// <summary>
		///   List of <see cref="GenericInfo"/> objects representing the generics of the component. Can be empty.
		/// </summary>
		public NamedTypeList<GenericInfo> Generics { get; private set; }

		/// <summary>
		///   List of <see cref="PortInfo"/> objects representing the ports of the component. Can be empty.
		/// </summary>
		public NamedTypeList<PortInfo> Ports { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="EntityInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the entity.</param>
		/// <param name="summary">Summary description of the entity.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/>, or <paramref name="summary"/> is an empty string.</exception>
		public EntityInfo(string name, string summary, string remarks = null)
			: base(name, summary, remarks)
		{
			Generics = new NamedTypeList<GenericInfo>();
			Ports = new NamedTypeList<PortInfo>();
		}

		/// <summary>
		///   Writes the entity to a stream.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the entity to.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">The number of ports and generics is zero.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public override void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			if (indentOffset < 0)
				indentOffset = 0;

			if (Generics.Count == 0 && Ports.Count == 0)
				throw new InvalidOperationException(string.Format("An attempt was made to write an entity ({0}), but the entity does not have any ports or generics.", Name));

			// Generate the documentation lookup table.
			Dictionary<string, string[]> lookup = new Dictionary<string, string[]>();
			lookup.Add("Summary", new string[] { Summary });

			if (Generics.Count > 0)
			{
				List<string> subItems = new List<string>(Generics.Count);
				foreach (GenericInfo info in Generics)
					subItems.Add(info.GetDocumentationString());
				lookup.Add("Generics", subItems.ToArray());
			}

			if(Ports.Count > 0)
			{
				List<string> subItems = new List<string>(Ports.Count);
				foreach (PortInfo info in Ports)
					subItems.Add(info.GetDocumentationString());
				lookup.Add("Ports", subItems.ToArray());
			}

			if (!string.IsNullOrWhiteSpace(Remarks))
				lookup.Add("Remarks", new string[] { Remarks });

			// Write the header.
			DocumentationHelper.WriteFlowerLine(wr, indentOffset);
			DocumentationHelper.WriteGeneralDocumentationElements(wr, lookup, indentOffset);
			DocumentationHelper.WriteFlowerLine(wr, indentOffset);
			DocumentationHelper.WriteLine(wr, string.Format("entity {0} is", Name), indentOffset);

			if (Generics.Count > 0)
				SimplifiedGenericInfo.WriteGenericDeclaration(wr, Generics.ToArray(), indentOffset);

			if (Ports.Count > 0)
				SimplifiedPortInfo.WritePortDeclaration(wr, Ports.ToArray(), indentOffset);

			StringBuilder sb = new StringBuilder();
			sb.Append("end");
			if (DefaultValues.AddOptionalTypeNames)
				sb.Append(" entity");
			if (DefaultValues.AddOptionalNames)
				sb.AppendFormat(" {0}", Name);
			sb.Append(";");
			DocumentationHelper.WriteLine(wr, sb.ToString(), indentOffset);
		}

		#endregion Methods
	}
}
