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
using System.Collections.Generic;

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
		public List<GenericInfo> Generics { get; private set; }

		/// <summary>
		///   List of <see cref="PortInfo"/> objects representing the ports of the component. Can be empty.
		/// </summary>
		public List<PortInfo> Ports { get; private set; }

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
			Generics = new List<GenericInfo>();
			Ports = new List<PortInfo>();
		}

		#endregion Methods
	}
}
