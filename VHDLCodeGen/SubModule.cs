//********************************************************************************************************************************
// Filename:    SubModule.cs
// Owner:       Richard Dunkley
// Description: Contains the data components for a VHDL component instantiation.
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
	///   Represents a VHDL sub-module.
	/// </summary>
	public class SubModule : BaseTypeInfo
	{
		#region Properties

		/// <summary>
		///   Component associated with the sub-module.
		/// </summary>
		public ComponentInfo Component { get; protected set; }

		/// <summary>
		///   Lookup table for the generic mapping.
		/// </summary>
		public NamedTypeLookup<SimplifiedGenericInfo, string> GenericMap { get; private set; }

		/// <summary>
		///   Lookup table for the port mapping.
		/// </summary>
		public NamedTypeLookup<SimplifiedPortInfo, string> PortMap { get; private set; }

		/// <summary>
		///   Lookup table for port conversions.
		/// </summary>
		public NamedTypeLookup<SimplifiedPortInfo, string> ConversionMap { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="SubModule"/>.
		/// </summary>
		/// <param name="name">Name of the sub-module</param>
		/// <param name="summary">Summary description of the sub-module.</param>
		/// <param name="component"><see cref="ComponentInfo"/> object representing the component of the sub-module.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="component"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/>, or <paramref name="summary"/> is an empty string.</exception>
		public SubModule(string name, string summary, ComponentInfo component, string remarks = null)
			: base(name, summary, remarks)
		{
			if (component == null)
				throw new ArgumentNullException("component");

			Component = component;

			GenericMap = new NamedTypeLookup<SimplifiedGenericInfo, string>(component.Generics);
			PortMap = new NamedTypeLookup<SimplifiedPortInfo, string>(component.Ports);
			ConversionMap = new NamedTypeLookup<SimplifiedPortInfo, string>(component.Ports);
		}

		/// <summary>
		///   Gets all the unique components contained in the sub-modules.
		/// </summary>
		/// <param name="subModules"><see cref="SubModule"/>s to pull unique <see cref="ComponentInfo"/> base objects from.</param>
		/// <returns>Array of unique components.</returns>
		/// <remarks>This method determines uniqueness by the instance not by the <see cref="SubModule"/>'s name.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="subModules"/> is a null reference.</exception>
		public static ComponentInfo[] GetUniqueComponents(IEnumerable<SubModule> subModules)
		{
			if (subModules == null)
				throw new ArgumentNullException("subModules");

			List<ComponentInfo> components = new List<ComponentInfo>();
			foreach (SubModule mod in subModules)
			{
				if (!components.Contains(mod.Component))
					components.Add(mod.Component);
			}
			return components.ToArray();
		}

		/// <summary>
		///   Validates that mappings have a one-to-one relationship.
		/// </summary>
		/// <exception cref="InvalidOperationException">The maps don't have a one-to-one relationship.</exception>
		private void ValidateMappings()
		{
			if (Component.Generics.Count != GenericMap.Count)
			{
				throw new InvalidOperationException(string.Format
				(
					"The sub-module ({0}), does not have the same amount of generic maps ({1}) as the associated component's ({2}) generics ({3}).",
					Name,
					GenericMap.Count.ToString(),
					Component.Name,
					Component.Generics.Count.ToString()
				));
			}

			foreach(SimplifiedGenericInfo info in Component.Generics)
			{
				if (!GenericMap.ContainsKey(info))
				{
					throw new InvalidOperationException(string.Format
					(
						"The sub-module ({0}), does not have a mapping for a generic ({1}) in the associated component ({2}).",
						Name,
						info.Name,
						Component.Name
					));
				}
			}

			if (Component.Ports.Count != PortMap.Count)
			{
				throw new InvalidOperationException(string.Format
				(
					"The sub-module ({0}), does not have the same amount of port maps ({1}) as the associated component's ({2}) ports ({3}).",
					Name,
					PortMap.Count.ToString(),
					Component.Name,
					Component.Ports.Count.ToString()
				));
			}

			foreach (SimplifiedPortInfo info in Component.Ports)
			{
				if (!PortMap.ContainsKey(info))
				{
					throw new InvalidOperationException(string.Format
					(
						"The sub-module ({0}), does not have a mapping for a port ({1}) in the associated component ({2}).",
						Name,
						info.Name,
						Component.Name
					));
				}
			}
		}

		/// <summary>
		///   Writes the sub-module to a stream.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the sub-module to.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">There was a problem with the mappings.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public override void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			if (indentOffset < 0)
				indentOffset = 0;

			// Validate the mappings.
			ValidateMappings();

			// Write the header.
			WriteBasicHeader(wr, indentOffset);
			DocumentationHelper.WriteLine(wr, string.Format("{0}: {1}", Name, Component.Name), indentOffset);

			if (GenericMap.Count > 0)
			{
				if(DefaultValues.AddSpaceAfterKeyWords)
					DocumentationHelper.WriteLine(wr, "generic map (", indentOffset);
				else
					DocumentationHelper.WriteLine(wr, "generic map(", indentOffset);

				// Write the generics out.
				int index = 0;
				string ending = ",";
				foreach (SimplifiedGenericInfo info in GenericMap.Keys)
				{
					if (index == GenericMap.Count - 1)
						ending = string.Empty;
					DocumentationHelper.WriteLine(wr, string.Format("{0} => {1}{2}", info.Name, GenericMap[info], ending), indentOffset + 1);
					index++;
				}

				if(PortMap.Count == 0)
					DocumentationHelper.WriteLine(wr, ");", indentOffset);
				else
					DocumentationHelper.WriteLine(wr, ")", indentOffset);
			}

			if (PortMap.Count > 0)
			{
				if(DefaultValues.AddSpaceAfterKeyWords)
					DocumentationHelper.WriteLine(wr, "port map (", indentOffset);
				else
					DocumentationHelper.WriteLine(wr, "port map(", indentOffset);

				// Write the ports out.
				int index = 0;
				string ending = ",";
				foreach (SimplifiedPortInfo info in PortMap.Keys)
				{
					if (index == PortMap.Count - 1)
						ending = string.Empty;
					if(ConversionMap[info] == null)
						DocumentationHelper.WriteLine(wr, string.Format("{0} => {1}{2}", info.Name, PortMap[info], ending), indentOffset + 1);
					else
						DocumentationHelper.WriteLine(wr, string.Format("{0}({1}) => {2}{3}", ConversionMap[info], info.Name, PortMap[info], ending), indentOffset + 1);
					index++;
				}

				DocumentationHelper.WriteLine(wr, ");", indentOffset);
			}
		}

		#endregion Methods
	}
}
