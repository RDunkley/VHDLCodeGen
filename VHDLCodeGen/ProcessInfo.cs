//********************************************************************************************************************************
// Filename:    ProcessInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components for a VHDL process.
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
	///   Represents a VHDL process.
	/// </summary>
	public class ProcessInfo : BaseTypeInfo
	{
		#region Properties

		/// <summary>
		///   List containing the lines of code of the process.
		/// </summary>
		public List<string> CodeLines;

		/// <summary>
		///   Sensitivity list of the process. Can be empty.
		/// </summary>
		public List<string> SensitivityList;

		/// <summary>
		///   List of <see cref="VariableInfo"/> objects representing the variables in the process. Can be empty.
		/// </summary>
		public List<VariableInfo> Variables;

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ProcessInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the process.</param>
		/// <param name="summary">Summary description of the process.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/>, or <paramref name="summary"/> is an empty string.</exception>
		public ProcessInfo(string name, string summary, string remarks = null)
			: base(name, summary, remarks)
		{
			SensitivityList = new List<string>();
			Variables = new List<VariableInfo>();
			CodeLines = new List<string>();
		}

		#endregion Methods
	}
}
