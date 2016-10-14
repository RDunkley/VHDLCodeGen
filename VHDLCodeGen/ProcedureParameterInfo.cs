//********************************************************************************************************************************
// Filename:    ProcedureParameterInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to describe a VHDL procedure parameter.
//********************************************************************************************************************************
// Copyright © Richard Dunkley 2016
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0  Unless required by applicable
// law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
// limitations under the License.
//********************************************************************************************************************************
namespace VHDLCodeGen
{
	/// <summary>
	///   Represents a VHDL procedure's parameter.
	/// </summary>
	public class ProcedureParameterInfo : ParameterInfo
	{
		#region Properties

		/// <summary>
		///   Direction of the parameter.
		/// </summary>
		public PortDirection Direction { get; protected set; }

		/// <summary>
		///   Type of parameter. Can be null.
		/// </summary>
		public ProcedureParameterType? ParameterType { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ProcedureParameterInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the parameter.</param>
		/// <param name="direction"><see cref="PortDirection"/> associated with the parameter.</param>
		/// <param name="type">Type of the parameter.</param>
		/// <param name="description">Description of the parameter.</param>
		/// <param name="parameterType"><see cref="ProcedureParameterType"/> associated with the parameter.</param>
		/// <exception cref="ArgumentNullException"><paramref name="type"/>, <paramref name="name"/>, or <paramref name="description"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/>, <paramref name="name"/>, or <paramref name="description"/> is an empty string.</exception>
		public ProcedureParameterInfo(string name, PortDirection direction, string type, string description, ProcedureParameterType? parameterType = null)
			: base(name, type, description)
		{
			Direction = direction;
			ParameterType = parameterType;
		}

		#endregion Methods
	}
}
