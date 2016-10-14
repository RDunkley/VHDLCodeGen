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
using System.Collections.Generic;

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
		public List<SimplifiedGenericInfo> Generics { get; private set; }

		/// <summary>
		///   List of <see cref="SimplifiedPortInfo"/> objects representing the ports of the component. Can be empty.
		/// </summary>
		public List<SimplifiedPortInfo> Ports { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ComponentInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the component.</param>
		/// <param name="summary">Summary description of the component.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/>, or <paramref name="summary"/> is an empty string.</exception>
		public ComponentInfo(string name, string summary, string remarks = null)
			: base(name, summary, remarks)
		{
			Generics = new List<SimplifiedGenericInfo>();
			Ports = new List<SimplifiedPortInfo>();
		}

		/// <summary>
		///   Instantiates a new <see cref="ComponentInfo"/> object using a <see cref="EntityInfo"/> object.
		/// </summary>
		/// <param name="info"><see cref="EntityInfo"/> object to pull the component information from.</param>
		/// <remarks>This constructor generates a component from an entity so that an entity can be used in another module.</remarks>
		public ComponentInfo(EntityInfo info)
			: base(info.Name, info.Summary, info.Remarks)
		{
			Generics = new List<SimplifiedGenericInfo>(info.Generics.Count);
			foreach (GenericInfo gen in info.Generics)
				Generics.Add(new SimplifiedGenericInfo(gen.Name, gen.Type, gen.DefaultValue));

			Ports = new List<SimplifiedPortInfo>(info.Ports.Count);
			foreach (PortInfo port in info.Ports)
				Ports.Add(new SimplifiedPortInfo(port.Name, port.Direction, port.BitWidth));
		}

		#endregion Methods

	}
}
