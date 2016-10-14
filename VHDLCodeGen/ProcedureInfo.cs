//********************************************************************************************************************************
// Filename:    ProcedureInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to describe a VHDL procedure.
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
	///   Represents the information of a VHDL procedure.
	/// </summary>
	public class ProcedureInfo : BaseTypeInfo
	{
		#region Properties

		/// <summary>
		///   List of the lines of code contained in the procedure.
		/// </summary>
		public List<string> CodeLines { get; private set; }

		/// <summary>
		///   List of <see cref="ProcedureParameterInfo"/> objects representing parameters passed into the procedure in order of
		///   appearance in signature.
		/// </summary>
		public List<ProcedureParameterInfo> Parameters { get; private set; }

		/// <summary>
		///   List of <see cref="VariableInfo"/> objects representing variables declared in the procedure. Can be empty.
		/// </summary>
		public List<VariableInfo> Variables { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ProcedureInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the procedure.</param>
		/// <param name="summary">Summary description of the procedure.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/>, or <paramref name="summary"/> is an empty string.</exception>
		public ProcedureInfo(string name, string summary, string remarks = null)
			: base(name, summary, remarks)
		{
			Parameters = new List<ProcedureParameterInfo>();
			Variables = new List<VariableInfo>();
			CodeLines = new List<string>();
		}

		#endregion Methods
	}
}
