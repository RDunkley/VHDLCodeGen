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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

		/// <summary>
		///   Writes the procedure to a stream.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the procedure to.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">No code lines were specified or no parameters were specified.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public override void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			if (indentOffset < 0)
				indentOffset = 0;

			if (CodeLines.Count == 0)
				throw new InvalidOperationException(string.Format("An attempt was made to write a procedure ({0}), but it doesn't have any code associated with it", Name));
			if(Parameters.Count == 0)
				throw new InvalidOperationException(string.Format("An attempt was made to write a procedure ({0}), but it doesn't have any parameters", Name));

			// Generate the documentation lookup table.
			Dictionary<string, string[]> lookup = new Dictionary<string, string[]>();
			lookup.Add("Summary", new string[] { Summary });

			if (Parameters.Count > 0)
			{
				List<string> subItems = new List<string>(Parameters.Count);
				foreach (ProcedureParameterInfo info in Parameters)
					subItems.Add(info.GetDocumentationString());
				lookup.Add("Parameters", subItems.ToArray());
			}

			if (!string.IsNullOrWhiteSpace(Remarks))
				lookup.Add("Remarks", new string[] { Remarks });

			// Write the header.
			DocumentationHelper.WriteFlowerLine(wr, indentOffset);
			DocumentationHelper.WriteGeneralDocumentationElements(wr, lookup, indentOffset);
			DocumentationHelper.WriteFlowerLine(wr, indentOffset);
			DocumentationHelper.WriteLine(wr, GetSignature(), indentOffset);

			// Write the variable declarations out.
			BaseTypeInfo.WriteBaseTypeInfos(null, wr, Variables.ToArray(), indentOffset + 1, Name, "procedure");

			DocumentationHelper.WriteLine(wr, "begin", indentOffset);

			// Write the code lines.
			foreach (string line in CodeLines)
				DocumentationHelper.WriteLine(wr, line, indentOffset + 1);

			DocumentationHelper.WriteLine(wr, "end procedure;", indentOffset);
		}

		/// <summary>
		///   Gets the signature of the procedure.
		/// </summary>
		/// <returns>Signature of the procedure.</returns>
		private string GetSignature()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("procedure {0}(", Name);
			for (int i = 0; i < Parameters.Count; i++)
			{
				sb.Append(Parameters[i].GetSignatureString());
				if (i != Parameters.Count - 1)
					sb.Append("; ");
			}
			sb.AppendFormat(")");
			return sb.ToString();
		}

		#endregion Methods
	}
}
