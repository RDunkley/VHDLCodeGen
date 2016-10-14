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
		#region Properties

		/// <summary>
		///   List of <see cref="AliasInfo"/> objects representing the aliases in the module. Can be empty.
		/// </summary>
		public List<AliasInfo> Aliases { get; private set; }

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
		public List<DeclarationInfo> DeclaredTypes { get; private set; }

		/// <summary>
		///   Entity associated with the module.
		/// </summary>
		public EntityInfo Entity { get; protected set; }

		/// <summary>
		///   List of <see cref="FunctionInfo"/> objects representing the functions in the module. Can be empty.
		/// </summary>
		public List<FunctionInfo> Functions { get; private set; }

		/// <summary>
		///   List of <see cref="GenerateInfo"/> objects representing the generate sections in the module. Can be empty.
		/// </summary>
		public List<GenerateInfo> Generates { get; private set; }

		/// <summary>
		///   List of <see cref="ProcedureInfo"/> objects representing the procedures in the module. Can be empty.
		/// </summary>
		public List<ProcedureInfo> Procedures { get; private set; }

		/// <summary>
		///   List of <see cref="ProcessInfo"/> objects representing the processes in the module. Can be empty.
		/// </summary>
		public List<ProcessInfo> Processes { get; private set; }

		/// <summary>
		///   List of <see cref="SignalInfo"/> objects representing the signals in the module. Can be empty.
		/// </summary>
		public List<SignalInfo> Signals { get; private set; }

		/// <summary>
		///   List of <see cref="SubModule"/> objects representing the instantiated components in the module. Can be empty.
		/// </summary>
		public List<SubModule> SubModules { get; private set; }

		/// <summary>
		///   Architectural type associated with the module.
		/// </summary>
		public ArchitecturalType Type { get; protected set; }

		/// <summary>
		///   List of use statements in the module (example: 'IEEE.STD_LOGIC_1164.all'). Can be empty.
		/// </summary>
		public List<string> Usings { get; private set; }

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

			DeclaredTypes = new List<DeclarationInfo>();
			Functions = new List<FunctionInfo>();
			Procedures = new List<ProcedureInfo>();
			Signals = new List<SignalInfo>();
			Aliases = new List<AliasInfo>();
			Attributes = new List<AttributeSpecificationInfo>();
			Processes = new List<ProcessInfo>();
			Generates = new List<GenerateInfo>();
			SubModules = new List<SubModule>();
			Usings = new List<string>();
			ConcurrentStatements = new List<string>();
		}

		#endregion Methods
	}
}
