//********************************************************************************************************************************
// Filename:    ModuleInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components of a VHDL module.
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
	///   Represents a VHDL module.
	/// </summary>
	/// 
	/// <remarks>
	///   This is a simplified module that represents an entity and its associated architecture, multiple configurations, etc. 
	///   are not currently supported.
	/// </remarks>
	public class ModuleInfo
	{
		#region Fields

		/// <summary>
		///   Tracks the usings associated with the module.
		/// </summary>
		private List<string> mUsingList = new List<string>();

		#endregion Fields

		#region Properties

		/// <summary>
		///   List of <see cref="AliasInfo"/> objects representing the aliases in the module. Can be empty.
		/// </summary>
		public NamedTypeList<AliasInfo> Aliases { get; private set; }

		/// <summary>
		///   List of <see cref="AttributeSpecificationInfo"/> objects representing the attributes in the module. Can be empty.
		/// </summary>
		public List<AttributeSpecificationInfo> Attributes { get; private set; }

		/// <summary>
		///   List of the concurrent lines of code in the module. Can be empty.
		/// </summary>
		public List<string> ConcurrentStatements { get; private set; }

		/// <summary>
		///   List of <see cref="DeclarationInfo"/> objects representing the declared types in the module. Can be empty.
		/// </summary>
		public NamedTypeList<DeclarationInfo> DeclaredTypes { get; private set; }

		/// <summary>
		///   Entity associated with the module.
		/// </summary>
		public EntityInfo Entity { get; protected set; }

		/// <summary>
		///   List of <see cref="FunctionInfo"/> objects representing the functions in the module. Can be empty.
		/// </summary>
		public NamedTypeList<FunctionInfo> Functions { get; private set; }

		/// <summary>
		///   List of <see cref="GenerateInfo"/> objects representing the generate sections in the module. Can be empty.
		/// </summary>
		public NamedTypeList<GenerateInfo> Generates { get; private set; }

		/// <summary>
		///   List of <see cref="ProcedureInfo"/> objects representing the procedures in the module. Can be empty.
		/// </summary>
		public NamedTypeList<ProcedureInfo> Procedures { get; private set; }

		/// <summary>
		///   List of <see cref="ProcessInfo"/> objects representing the processes in the module. Can be empty.
		/// </summary>
		public NamedTypeList<ProcessInfo> Processes { get; private set; }

		/// <summary>
		///   List of <see cref="SignalInfo"/> objects representing the signals in the module. Can be empty.
		/// </summary>
		public NamedTypeList<SignalInfo> Signals { get; private set; }

		/// <summary>
		///   List of <see cref="SubModule"/> objects representing the instantiated components in the module. Can be empty.
		/// </summary>
		public NamedTypeList<SubModule> SubModules { get; private set; }

		/// <summary>
		///   Architectural type associated with the module.
		/// </summary>
		public ArchitecturalType Type { get; protected set; }

		/// <summary>
		///   Array of use statements in the module (example: 'IEEE.STD_LOGIC_1164.all'). Can be empty.
		/// </summary>
		public string[] Usings
		{
			get
			{
				return mUsingList.ToArray();
			}
		}

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ModuleInfo"/> object.
		/// </summary>
		/// <param name="type"><see cref="ArchitecturalType"/> of the module.</param>
		/// <param name="entity"><see cref="EntityInfo"/> object representing the associated entity for this module.</param>
		/// <exception cref="ArgumentNullException"><paramref name="entity"/> is a null reference.</exception>
		public ModuleInfo(ArchitecturalType type, EntityInfo entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			Type = type;
			Entity = entity;

			DeclaredTypes = new NamedTypeList<DeclarationInfo>();
			Functions = new NamedTypeList<FunctionInfo>();
			Procedures = new NamedTypeList<ProcedureInfo>();
			Signals = new NamedTypeList<SignalInfo>();
			Aliases = new NamedTypeList<AliasInfo>();
			Attributes = new List<AttributeSpecificationInfo>();
			Processes = new NamedTypeList<ProcessInfo>();
			Generates = new NamedTypeList<GenerateInfo>();
			SubModules = new NamedTypeList<SubModule>();
			ConcurrentStatements = new List<string>();

			// Add the default usings.
			AddUsing("IEEE.STD_LOGIC_1164.all");
			AddUsing("IEEE.NUMERIC_STD.all");
		}

		/// <summary>
		///   Validates that all the child names are unique.
		/// </summary>
		/// <exception cref="InvalidOperationException">Two child objects were found with the same name.</exception>
		private void ValidateChildNames()
		{
			List<BaseTypeInfo> childList = new List<BaseTypeInfo>();
			childList.AddRange(DeclaredTypes);
			childList.AddRange(Functions);
			childList.AddRange(Procedures);
			childList.AddRange(SubModule.GetUniqueComponents(SubModules));
			childList.AddRange(Signals);
			childList.AddRange(Aliases);
			childList.AddRange(AttributeDeclarationInfo.GetUniqueAttributeDeclarations(Attributes));
			childList.AddRange(Processes);
			childList.AddRange(Generates);
			childList.AddRange(SubModules);

			BaseTypeInfo.ValidateNoDuplicates(childList.ToArray(), Entity.Name, "module");
		}

		/// <summary>
		///   Writes the module to a stream.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the entity to.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">Unable to write the object out in its current state.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			if (indentOffset < 0)
				indentOffset = 0;

			ValidateChildNames();

			// Write the entity.
			Entity.Write(wr, indentOffset);

			DocumentationHelper.WriteLine(wr);
			DocumentationHelper.WriteLine(wr, string.Format("architecture {0} of {1} is", Enum.GetName(typeof(ArchitecturalType), Type), Entity.Name), indentOffset);
			DocumentationHelper.WriteLine(wr);

			// Functions
			BaseTypeInfo.WriteBaseTypeInfos("Functions", wr, Functions.ToArray(), indentOffset, Entity.Name, "module");
			if (Functions.Count > 0)
				DocumentationHelper.WriteLine(wr);

			// Types and Constants
			DeclarationInfo.WriteDeclarations(wr, DeclaredTypes.ToArray(), indentOffset, Entity.Name, "module");
			if(DeclaredTypes.Count > 0)
				DocumentationHelper.WriteLine(wr);

			// Procedures
			BaseTypeInfo.WriteBaseTypeInfos("Procedures", wr, Procedures.ToArray(), indentOffset, Entity.Name, "module");
			if (Procedures.Count > 0)
				DocumentationHelper.WriteLine(wr);

			// Components
			ComponentInfo[] components = SubModule.GetUniqueComponents(SubModule.GetAllSubModules(this));
			BaseTypeInfo.WriteBaseTypeInfos("Components", wr, components, indentOffset, Entity.Name, "module");
			if (components.Length > 0)
				DocumentationHelper.WriteLine(wr);

			// Signals
			BaseTypeInfo.WriteBaseTypeInfos("Signals", wr, Signals.ToArray(), indentOffset, Entity.Name, "module");
			if (Signals.Count > 0)
				DocumentationHelper.WriteLine(wr);

			// Aliases
			BaseTypeInfo.WriteBaseTypeInfos("Aliases", wr, Aliases.ToArray(), indentOffset, Entity.Name, "module");
			if (Aliases.Count > 0)
				DocumentationHelper.WriteLine(wr);

			// Attributes
			AttributeSpecificationInfo.WriteAttributes(wr, Attributes.ToArray(), indentOffset, Entity.Name, "module");
			if(Attributes.Count > 0)
				DocumentationHelper.WriteLine(wr);

			DocumentationHelper.WriteLine(wr, "begin", indentOffset);
			DocumentationHelper.WriteLine(wr);

			// Concurrent Statements
			foreach (string line in ConcurrentStatements)
				DocumentationHelper.WriteLine(wr, line, indentOffset);
			if(ConcurrentStatements.Count > 0)
				DocumentationHelper.WriteLine(wr);

			// Processes
			BaseTypeInfo.WriteBaseTypeInfos("Processes", wr, Processes.ToArray(), indentOffset, Entity.Name, "module");
			if (Processes.Count > 0)
				DocumentationHelper.WriteLine(wr);

			// Generates
			BaseTypeInfo.WriteBaseTypeInfos("Generates", wr, Generates.ToArray(), indentOffset, Entity.Name, "module");
			if (Generates.Count > 0)
				DocumentationHelper.WriteLine(wr);

			BaseTypeInfo.WriteBaseTypeInfos("Sub-Modules", wr, SubModules.ToArray(), indentOffset, Entity.Name, "module");
			if (SubModules.Count > 0)
				DocumentationHelper.WriteLine(wr);

			StringBuilder sb = new StringBuilder();
			sb.Append("end");
			if (DefaultValues.AddOptionalTypeNames)
				sb.Append(" architecture");
			if (DefaultValues.AddOptionalNames)
				sb.AppendFormat(" {0}", Enum.GetName(typeof(ArchitecturalType), Type));
			sb.Append(";");
			DocumentationHelper.Write(wr, sb.ToString(), indentOffset);
		}

		/// <summary>
		///   Adds a list of usings to the Usings of this <see cref="NamespaceTypeInfo"/> object.
		/// </summary>
		/// <param name="usings">Array of usings to be added.</param>
		public void AddUsings(string[] usings)
		{
			// Add any usings that are not duplicates.
			foreach (string usingString in usings)
			{
				if (!mUsingList.Contains(usingString))
					mUsingList.Add(usingString);
			}

			// Sort the using list.
			if (mUsingList.Count > 0)
				mUsingList.Sort();
		}

		/// <summary>
		///   Add an additonal using to the Usings of this <see cref="NamespaceTypeInfo"/> object.
		/// </summary>
		/// <param name="usingString">Using string to be added.</param>
		public void AddUsing(string usingString)
		{
			if (mUsingList.Contains(usingString))
				return;

			mUsingList.Add(usingString);

			// Sort the using list.
			if (mUsingList.Count > 0)
				mUsingList.Sort();
		}

		/// <summary>
		///   Remove the specified using from this <see cref="NamespaceTypeInfo"/> object.
		/// </summary>
		/// <param name="usingString">Using string to be removed.</param>
		public void RemoveUsing(string usingString)
		{
			if (!mUsingList.Contains(usingString))
				return;

			mUsingList.Remove(usingString);

			// Sort the using list.
			if (mUsingList.Count > 0)
				mUsingList.Sort();
		}

		#endregion Methods
	}
}
