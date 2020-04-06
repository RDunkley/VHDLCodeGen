//********************************************************************************************************************************
// Filename:    ComponentInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components for a VHDL component.
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
	///   Represents a VHDL component.
	/// </summary>
	public class ComponentInfo : BaseTypeInfo
	{
		#region Properties

		/// <summary>
		///   List of <see cref="SimplifiedGenericInfo"/> objects representing the generics of the component. Can be empty.
		/// </summary>
		public NamedTypeList<SimplifiedGenericInfo> Generics { get; private set; }

		/// <summary>
		///   List of <see cref="SimplifiedPortInfo"/> objects representing the ports of the component. Can be empty.
		/// </summary>
		public NamedTypeList<SimplifiedPortInfo> Ports { get; private set; }

		/// <summary>
		///   True if the declaration of the component should be skipped, false if it should be created.
		/// </summary>
		public bool SkipDeclaration { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ComponentInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the component.</param>
		/// <param name="summary">Summary description of the component.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <param name="skipDeclaration">
		///   True if the component should not be declared in the module, false if it should. True would be specified for components that
		///   represent internal macros, primitives, or sub-modules that will be declared directly.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/>, or <paramref name="summary"/> is an empty string.</exception>
		public ComponentInfo(string name, string summary, string remarks = null, bool skipDeclaration = false)
			: base(name, summary, remarks)
		{
			Generics = new NamedTypeList<SimplifiedGenericInfo>();
			Ports = new NamedTypeList<SimplifiedPortInfo>();
			SkipDeclaration = skipDeclaration;
		}

		/// <summary>
		///   Instantiates a new <see cref="ComponentInfo"/> object using a <see cref="EntityInfo"/> object.
		/// </summary>
		/// <param name="info"><see cref="EntityInfo"/> object to pull the component information from.</param>
		/// <param name="skipDeclaration">
		///   True if the component should not be declared in the module, false if it should. True would be specified for components that
		///   represent internal macros, primitives, or sub-modules that will be declared directly.
		/// </param>
		/// <remarks>This constructor generates a component from an entity so that an entity can be used in another module.</remarks>
		public ComponentInfo(EntityInfo info, bool skipDeclaration = false)
			: base(info.Name, info.Summary, info.Remarks)
		{
			Generics = new NamedTypeList<SimplifiedGenericInfo>(info.Generics.Count);
			foreach (GenericInfo gen in info.Generics)
				Generics.Add(new SimplifiedGenericInfo(gen.Name, gen.Type, gen.DefaultValue));

			Ports = new NamedTypeList<SimplifiedPortInfo>(info.Ports.Count);
			foreach (PortInfo port in info.Ports)
				Ports.Add(new SimplifiedPortInfo(port.Name, port.Direction, port.Type, port.DefaultValue));
			SkipDeclaration = skipDeclaration;
		}

		/// <summary>
		///   Writes the component to a stream.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the component to.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">No generics or ports were specified.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public override void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			if (SkipDeclaration)
				return;

			if (indentOffset < 0)
				indentOffset = 0;

			if (Generics.Count == 0 && Ports.Count == 0)
				throw new InvalidOperationException(string.Format("An attempt was made to write a component ({0}), but it doesn't have any ports or generics defined.", Name));

			// Write the header.
			WriteBasicHeader(wr, indentOffset);
			DocumentationHelper.WriteLine(wr, string.Format("component {0} is", Name), indentOffset);

			if (Generics.Count > 0)
				SimplifiedGenericInfo.WriteGenericDeclaration(wr, Generics.ToArray(), indentOffset);

			if (Ports.Count > 0)
				SimplifiedPortInfo.WritePortDeclaration(wr, Ports.ToArray(), indentOffset);

			StringBuilder sb = new StringBuilder();
			sb.Append("end component");
			if (DefaultValues.AddOptionalNames)
				sb.AppendFormat(" {0}", Name);
			sb.Append(";");
			DocumentationHelper.WriteLine(wr, sb.ToString(), indentOffset);
		}

		#endregion Methods
	}
}
