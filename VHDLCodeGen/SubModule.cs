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
		public Dictionary<SimplifiedGenericInfo, string> GenericMap { get; private set; }

		/// <summary>
		///   Lookup table for the port mapping.
		/// </summary>
		public Dictionary<SimplifiedPortInfo, string> PortMap { get; private set; }

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

			GenericMap = new Dictionary<SimplifiedGenericInfo, string>(component.Generics.Count);
			foreach (SimplifiedGenericInfo gen in component.Generics)
				GenericMap.Add(gen, null);

			PortMap = new Dictionary<SimplifiedPortInfo, string>(component.Ports.Count);
			foreach (SimplifiedPortInfo port in component.Ports)
				PortMap.Add(port, null);
		}

		#endregion Methods
	}
}
