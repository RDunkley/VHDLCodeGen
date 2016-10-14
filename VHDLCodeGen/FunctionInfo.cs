//********************************************************************************************************************************
// Filename:    FunctionInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to describe a VHDL function.
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
	///   Represents the information of a VHDL function.
	/// </summary>
	public class FunctionInfo : BaseTypeInfo
	{
		#region Properties

		/// <summary>
		///   List of the lines of code contained in the function.
		/// </summary>
		public List<string> CodeLines { get; private set; }

		/// <summary>
		///   List of <see cref="ParameterInfo"/> objects representing parameters passed into the funtion in order of appearance
		///   in signature. Can be empty.
		/// </summary>
		public List<ParameterInfo> Parameters { get; private set; }

		/// <summary>
		///   Return type of the function.
		/// </summary>
		public string ReturnType { get; protected set; }

		/// <summary>
		///   Summary description of the return type of the function.
		/// </summary>
		public string ReturnTypeSummary { get; set; }

		/// <summary>
		///   List of <see cref="VariableInfo"/> objects representing variables declared in the function. Can be empty.
		/// </summary>
		public List<VariableInfo> Variables { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a <see cref="FunctionInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the function.</param>
		/// <param name="returnType">Type of the return value.</param>
		/// <param name="summary">Summary description of the function.</param>
		/// <param name="returnTypeSummary">Summary description of the return type.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="returnTypeSummary"/>, <paramref name="returnType"/>, <paramref name="name"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="returnTypeSummary"/>, <paramref name="returnType"/>, <paramref name="name"/>, or <paramref name="summary"/> is an empty string.</exception>
		public FunctionInfo(string name, string returnType, string summary, string returnTypeSummary, string remarks = null)
			: base(name, summary, remarks)
		{
			if (returnType == null)
				throw new ArgumentNullException("returnType");
			if (returnType.Length == 0)
				throw new ArgumentException("returnType is an empty string");
			if (returnTypeSummary == null)
				throw new ArgumentNullException("returnTypeSummary");
			if (returnTypeSummary.Length == 0)
				throw new ArgumentException("returnTypeSummary is an empty string");

			ReturnType = returnType;
			ReturnTypeSummary = returnTypeSummary;
			Parameters = new List<ParameterInfo>();
			Variables = new List<VariableInfo>();
			CodeLines = new List<string>();
		}

		#endregion Methods
	}
}
