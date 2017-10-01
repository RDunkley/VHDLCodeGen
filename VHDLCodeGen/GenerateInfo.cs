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
using System.IO;

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
		public NamedTypeList<GenerateInfo> Generates { get; private set; }

		/// <summary>
		///   Generate statement.
		/// </summary>
		/// <remarks>This is the statement made to start the generate block (minus the actual generate key word). For example: 'if i < SOME_NUM' or 'for i in 0 to C_ARRAY_SIZE'.</remarks>
		public string GenerateStatement { get; protected set; }

		/// <summary>
		///   List of the <see cref="ProcessInfo"/> objects representing the processes defined inside the generate block. Can be empty.
		/// </summary>
		public NamedTypeList<ProcessInfo> Processes { get; private set; }

		/// <summary>
		///   List of the <see cref="SubModule"/> objects representing the sub-modules defined inside the generate block. Can be empty.
		/// </summary>
		public NamedTypeList<SubModule> SubModules { get; private set; }

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
			Processes = new NamedTypeList<ProcessInfo>();
			SubModules = new NamedTypeList<SubModule>();
			Generates = new NamedTypeList<GenerateInfo>();
		}

		/// <summary>
		///   Writes the process to a stream.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the process to.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">Unable to write the object out in its current state.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public override void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			if (indentOffset < 0)
				indentOffset = 0;

			if (ConcurrentStatements.Count == 0 && Processes.Count == 0 && SubModules.Count == 0 && Generates.Count == 0)
				throw new InvalidOperationException(string.Format("An attempt was made to write a generate section ({0}), but the section doesn't have anything to write (processes, sub-modules, etc.).", Name));

			// Validate that there are not duplicate names in the children.
			ValidateChildNames();

			// Validate that there is not an infinite loop.
			List<GenerateInfo> parentList = new List<GenerateInfo>();
			parentList.Add(this);
			ValidateChildGenerates(parentList);

			// Write the header.
			WriteBasicHeader(wr, indentOffset);
			DocumentationHelper.WriteLine(wr, string.Format("{0}:", Name), indentOffset);
			DocumentationHelper.WriteLine(wr, string.Format("{0} generate", GenerateStatement), indentOffset);
			indentOffset++;

			// Write the code lines.
			foreach (string line in ConcurrentStatements)
				DocumentationHelper.WriteLine(wr, line, indentOffset);

			if (ConcurrentStatements.Count > 0)
				DocumentationHelper.WriteLine(wr);

			// Write the processes.
			BaseTypeInfo.WriteBaseTypeInfos("Processes", wr, Processes.ToArray(), indentOffset, Name, "generate");
			if (Processes.Count > 0)
				DocumentationHelper.WriteLine(wr);

			// Write the generates.
			BaseTypeInfo.WriteBaseTypeInfos("Generates", wr, Generates.ToArray(), indentOffset, Name, "generate");
			if (Generates.Count > 0)
				DocumentationHelper.WriteLine(wr);

			// Write the sub-modules.
			BaseTypeInfo.WriteBaseTypeInfos("Sub-Modules", wr, SubModules.ToArray(), indentOffset, Name, "generate");

			indentOffset--;
			DocumentationHelper.WriteLine(wr, "end generate;", indentOffset);
		}

		/// <summary>
		///   Validates that all the child names are unique.
		/// </summary>
		/// <exception cref="InvalidOperationException">Two child objects were found with the same name.</exception>
		private void ValidateChildNames()
		{
			List<BaseTypeInfo> childList = new List<BaseTypeInfo>(Generates.Count + Processes.Count + SubModules.Count);
			foreach (ProcessInfo info in Processes)
				childList.Add(info);
			foreach (GenerateInfo info in Generates)
				childList.Add(info);
			foreach (SubModule info in SubModules)
				childList.Add(info);

			BaseTypeInfo.ValidateNoDuplicates(childList.ToArray(), Name, "generate");
		}

		/// <summary>
		///   Validates that none of the posterity of this <see cref="GenerateInfo"/> object is in the list or parent objects.
		/// </summary>
		/// <param name="parentList">List of <see cref="GenerateInfo"/> objects to validate.</param>
		public void ValidateChildGenerates(List<GenerateInfo> parentList)
		{
			// Check for infinite loops.
			foreach(GenerateInfo info in Generates)
			{
				if (parentList.Contains(info))
					throw new InvalidOperationException(string.Format("A generate object ({0}) contains itself as a child (meaning this will cause an infinite loop).", info.Name));

				// Add the child as a parent.
				parentList.Add(info);

				// Check it's child.
				info.ValidateChildGenerates(parentList);

				// Remove the parent (this allows duplicates at the same level farther down and duplicates at this level are validated with another method).
				parentList.Remove(info);
			}
		}

		#endregion Methods
	}
}
