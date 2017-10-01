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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
		public NamedTypeList<VariableInfo> Variables;

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
			Variables = new NamedTypeList<VariableInfo>();
			CodeLines = new List<string>();
		}

		/// <summary>
		///   Writes the process to a stream.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the process to.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">No code lines were specified.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public override void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			if (indentOffset < 0)
				indentOffset = 0;

			if (CodeLines.Count == 0)
				throw new InvalidOperationException(string.Format("An attempt was made to write a process ({0}), but the processes doesn't have any code associated with it", Name));

			// Write the header.
			WriteBasicHeader(wr, indentOffset);
			DocumentationHelper.WriteLine(wr, string.Format("{0}:", Name), indentOffset);
			DocumentationHelper.WriteLine(wr, GetSensitivityListLine(), indentOffset);

			// Write the variable declarations out.
			BaseTypeInfo.WriteBaseTypeInfos(null, wr, Variables.ToArray(), indentOffset + 1, Name, "process");

			DocumentationHelper.WriteLine(wr, "begin", indentOffset);

			// Write the code lines.
			foreach (string line in CodeLines)
				DocumentationHelper.WriteLine(wr, line, indentOffset + 1);

			DocumentationHelper.WriteLine(wr, "end process;", indentOffset);
		}

		/// <summary>
		///   Gets the sensitivity line.
		/// </summary>
		/// <returns>Sensitivity line of a process.</returns>
		private string GetSensitivityListLine()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("process(");
			for(int i = 0; i < SensitivityList.Count; i++)
			{
				sb.Append(SensitivityList[i]);
				if (i != SensitivityList.Count - 1)
					sb.Append(", ");
			}
			sb.Append(")");
			return sb.ToString();
		}

		#endregion Methods
	}
}
