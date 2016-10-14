//********************************************************************************************************************************
// Filename:    GenerateInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components for a VHDL generate section.
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
	///   Represents a generate section in a VHDL module.
	/// </summary>
	public class GenerateInfo : BaseTypeInfo
	{
		#region Properties

		/// <summary>
		///   List of the concurrent statements made in the generate section. Can be empty.
		/// </summary>
		public List<string> ConcurrentStatements { get; private set; }

		/// <summary>
		///   List of the <see cref="GenerateInfo"/> objects representing the generate blocks inside this generate block. Can be empty.
		/// </summary>
		public List<GenerateInfo> Generates { get; private set; }

		/// <summary>
		///   Generate statement.
		/// </summary>
		/// <remarks>This is the statement made to start the generate block (minus the actual generate key word). For example: 'if i < SOME_NUM' or 'for i in 0 to C_ARRAY_SIZE'.</remarks>
		public string GenerateStatement { get; protected set; }

		/// <summary>
		///   List of the <see cref="ProcessInfo"/> objects representing the processes defined inside the generate block. Can be empty.
		/// </summary>
		public List<ProcessInfo> Processes { get; private set; }

		/// <summary>
		///   List of the <see cref="SubModule"/> objects representing the sub-modules defined inside the generate block. Can be empty.
		/// </summary>
		public List<SubModule> SubModules { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a <see cref="GenerateInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the generate block.</param>
		/// <param name="summary">Summary description of the generate block.</param>
		/// <param name="generateStatement">Initial statement of the generate block (minus the generate key word).</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="generateStatement"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/>, <paramref name="generateStatement"/>, or <paramref name="summary"/> is an empty string.</exception>
		public GenerateInfo(string name, string summary, string generateStatement, string remarks = null)
			: base(name, summary, remarks)
		{
			if (generateStatement == null)
				throw new ArgumentNullException("generateStatement");
			if (generateStatement.Length == 0)
				throw new ArgumentException("generateStatement is an empty string");

			GenerateStatement = generateStatement;
			ConcurrentStatements = new List<string>();
			Processes = new List<ProcessInfo>();
			SubModules = new List<SubModule>();
			Generates = new List<GenerateInfo>();
		}

		#endregion Methods
	}
}
