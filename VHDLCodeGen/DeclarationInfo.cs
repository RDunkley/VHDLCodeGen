//********************************************************************************************************************************
// Filename:    DeclarationInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components of a constant, type, or subtype declaration in an architectural definition of an
//              entity.
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
	///   A declared type in the VHDL module.
	/// </summary>
	public class DeclarationInfo : BaseTypeInfo
	{
		#region Properties

		/// <summary>
		///   Type of the declaration or the declaration.
		/// </summary>
		public DeclarationType Declaration { get; protected set; }

		/// <summary>
		///   Default value of the declaration. Can be null or empty.
		/// </summary>
		/// 
		/// <remarks>Only valid if the <see cref="Type"/> is set to <see cref="DeclarationType.Constant"/>.</remarks>
		public string DefaultValue { get; set; }

		/// <summary>
		///   List of <see cref="DeclarationInfo"/> objects that this object is dependent upon. Can be empty.
		/// </summary>
		public NamedTypeList<DeclarationInfo> Dependency { get; private set; }

		/// <summary>
		///   Type of the declaration.
		/// </summary>
		public string Type { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="DeclarationInfo"/> object.
		/// </summary>
		/// <param name="declaration"><see cref="DeclarationType"/> of the declaration.</param>
		/// <param name="name">Name of the declaration.</param>
		/// <param name="type">Type of the declaration.</param>
		/// <param name="summary">Summary description of the declaration.</param>
		/// <param name="defaultValue">Default value of the declaration. Can be null or empty.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="type"/>, <paramref name="name"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/>, <paramref name="name"/>, or <paramref name="summary"/> is an empty string.</exception>
		/// <exception cref="InvalidOperationException"><paramref name="defaultValue"/> is specified when the <paramref name="declaration"/> is not a <see cref="DeclarationType.Constant"/>.</exception>
		public DeclarationInfo(DeclarationType declaration, string name, string type, string summary, string defaultValue = null, string remarks = null)
			: base(name, summary, remarks)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.Length == 0)
				throw new ArgumentException("type is an empty string");

			if(defaultValue != null && defaultValue.Length > 0 && declaration != DeclarationType.Constant)
			{
				throw new InvalidOperationException(string.Format
				(
					"An attempt was made to instantiate a DeclarationInfo object with a default value ({0}), for a non-constant ({1}). Only constants can have default values.",
					defaultValue,
					Enum.GetName(typeof(DeclarationType), declaration)
				));
			}

			Declaration = declaration;
			Type = type;
			DefaultValue = defaultValue;
			Dependency = new NamedTypeList<DeclarationInfo>();
		}


		/// <summary>
		///   Writes the declaration to a stream.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the declaration to.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">An attempt was made to write a default value to a non-constant.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public override void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			if (indentOffset < 0)
				indentOffset = 0;

			if (DefaultValue != null && DefaultValue.Length > 0 && Declaration != DeclarationType.Constant)
			{
				throw new InvalidOperationException(string.Format
				(
					"An attempt was made to write the DeclarationInfo object with a default value ({0}), for a non-constant ({1}). Only constants can have default values.",
					DefaultValue,
					Enum.GetName(typeof(DeclarationType), Declaration)
				));
			}

			string typeAssigner = "is";
			if (Declaration == DeclarationType.Constant)
				typeAssigner = ":";

			string defaultValueString = string.Empty;
			if (!string.IsNullOrWhiteSpace(DefaultValue))
				defaultValueString = string.Format(" := {0}", DefaultValue);

			// Write the header.
			WriteBasicHeader(wr, indentOffset);
			DocumentationHelper.WriteLine(wr, string.Format("{0} {1} {2} {3}{4};", Enum.GetName(typeof(DeclarationType), Declaration).ToLower(), Name, typeAssigner, Type, defaultValueString), indentOffset);
		}

		/// <summary>
		///   Sorts the declarations according to type and dependencies.
		/// </summary>
		/// <param name="declarations">Array of <see cref="DeclarationInfo"/> objects to sort.</param>
		/// <returns>Sorted array of <see cref="DeclarationInfo"/> objects provided.</returns>
		/// <exception cref="InvalidOperationException">A circular dependency exists in the <paramref name="declarations"/>.</exception>
		private static DeclarationInfo[] SortDeclarations(DeclarationInfo[] declarations)
		{
			// Place in buckets according to type.
			Dictionary<DeclarationType, List<DeclarationInfo>> lookup = new Dictionary<DeclarationType, List<DeclarationInfo>>();
			lookup.Add(DeclarationType.Constant, new List<DeclarationInfo>());
			lookup.Add(DeclarationType.SubType, new List<DeclarationInfo>());
			lookup.Add(DeclarationType.Type, new List<DeclarationInfo>());
			foreach (DeclarationInfo dec in declarations)
				lookup[dec.Declaration].Add(dec);

			// Create a list of the types in sort order.
			DeclarationType[] types = new DeclarationType[] { DeclarationType.Constant, DeclarationType.SubType, DeclarationType.Type };

			// Make multiple passes until all are placed (this should not hang as long as no circular dependencies exist).
			List<DeclarationInfo> sorted = new List<DeclarationInfo>(declarations.Length);
			bool nonePlaced = true;
			while (lookup[DeclarationType.Constant].Count != 0 || lookup[DeclarationType.SubType].Count != 0
				|| lookup[DeclarationType.Type].Count != 0)
			{
				nonePlaced = true;

				// Go through each type looking for items to place.
				for (int i = 0; i < types.Length; i++)
				{
					lookup[types[i]].Sort();
					List<DeclarationInfo> placed = new List<DeclarationInfo>();
					foreach (DeclarationInfo dec in lookup[types[i]])
					{
						if (CheckDependenciesAreIncluded(dec, sorted))
						{
							// Item is added if it has no dependencies or if the dependencies have all been added.
							sorted.Add(dec);
							placed.Add(dec);
							nonePlaced = false;
						}
					}

					// Remove placed items from the lookup.
					foreach (DeclarationInfo dec in placed)
						lookup[types[i]].Remove(dec);
				}

				if(nonePlaced && (lookup[DeclarationType.Constant].Count != 0 || lookup[DeclarationType.SubType].Count != 0
					|| lookup[DeclarationType.Type].Count != 0))
				{
					throw new InvalidOperationException("A Circular dependency was found in the declarations");
				}
			}

			return sorted.ToArray();
		}

		/// <summary>
		///   Checks that all the dependencies of <paramref name="info"/> are included in the <paramref name="list"/>.
		/// </summary>
		/// <param name="info"><see cref="DeclarationInfo"/> object to check dependencies of.</param>
		/// <param name="list">List of <see cref="DeclarationInfo"/> objects to validate against.</param>
		/// <returns>True if all were contained in the list, or no dependencies were found, false if a dependency was found that wasn't in the list.</returns>
		private static bool CheckDependenciesAreIncluded(DeclarationInfo info, List<DeclarationInfo> list)
		{
			if (info.Dependency.Count == 0)
				return true;

			foreach(DeclarationInfo dep in info.Dependency)
			{
				if (!list.Contains(dep))
					return false;

				if (!CheckDependenciesAreIncluded(dep, list))
					return false;
			}
			return true;
		}

		/// <summary>
		///   Validiates that there is not a circular dependency.
		/// </summary>
		/// <param name="info"><see cref="DeclarationInfo"/> to check for a circular dependency.</param>
		/// <param name="descendents">Chain of descendents that are dependent on <paramref name="info"/>.</param>
		/// <exception cref="InvalidOperationException"><paramref name="info"/> is a dependency of itself.</exception>
		private static void ValidateDependencies(DeclarationInfo info, DeclarationInfo[] descendents)
		{
			// Validate that this object is not in the chain.
			List<DeclarationInfo> checkList = new List<DeclarationInfo>(descendents);
			if (checkList.Contains(info))
				throw new InvalidOperationException(string.Format("The DeclarationInfo object ({0}) contains a dependency of itself.", info.Name));

			// Add this object to the chain.
			checkList.Add(info);

			// Validate it's parents.
			DeclarationInfo[] newArray = checkList.ToArray();
			foreach(DeclarationInfo child in info.Dependency)
				ValidateDependencies(child, newArray);
		}

		/// <summary>
		///   Validates that all the dependencies exist in the given list.
		/// </summary>
		/// <param name="info"><see cref="DeclarationInfo"/> object to validate the dependencies of.</param>
		/// <param name="declarations">List of <see cref="DeclarationInfo"/> to validate against.</param>
		/// <exception cref="InvalidOperationException">Occurs if one of the dependencies of <paramref name="info"/> doesn't exist in <paramref name="declarations"/>.</exception>
		private static void ValidateDependenciesExist(DeclarationInfo info, List<DeclarationInfo> declarations)
		{
			foreach(DeclarationInfo child in info.Dependency)
			{
				// Check that the child exists.
				if (!declarations.Contains(child))
					throw new InvalidOperationException(string.Format("The DeclarationInfo object ({0}) has a dependency ({0}) that does not exist.", info.Name, child.Name));

				// Check that any dependencies of the child exist.
				ValidateDependenciesExist(child, declarations);
			}
		}

		/// <summary>
		///   Writes the declarations to a stream.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the declarations to.</param>
		/// <param name="declarations">Array of <see cref="DeclarationInfo"/> objects to write to the stream.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <param name="parentName">Name of the parent object.</param>
		/// <param name="parentType">Type of the parent object.</param>
		/// <remarks>Use this method over the base class method to perform additional checks and better sorting.</remarks>
		/// <exception cref="ArgumentException"><paramref name="parentName"/>, or <paramref name="parentType"/> is an empty string.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/>, <paramref name="parentName"/>, or <paramref name="parentType"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">A circular dependency exists or duplicate name is found in the <paramref name="declarations"/>.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static void WriteDeclarations(StreamWriter wr, DeclarationInfo[] declarations, int indentOffset, string parentName, string parentType)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (declarations == null)
				throw new ArgumentNullException("declarations");
			if (parentName == null)
				throw new ArgumentNullException("parentName");
			if (parentName.Length == 0)
				throw new ArgumentException("parentName is an empty string");
			if (parentType == null)
				throw new ArgumentNullException("parentType");
			if (parentType.Length == 0)
				throw new ArgumentException("parentType is an empty string");

			if (indentOffset < 0)
				indentOffset = 0;

			// Write nothing if array is empty.
			if (declarations.Length == 0)
				return;

			// Validate that we don't have duplicate names.
			BaseTypeInfo.ValidateNoDuplicates(declarations, parentName, parentType);

			// Validate that no circular dependencies exist.
			foreach(DeclarationInfo dec in declarations)
				ValidateDependencies(dec, new DeclarationInfo[0]);

			// Validate that all dependencies are in the main list.
			List<DeclarationInfo> decList = new List<DeclarationInfo>(declarations);
			foreach (DeclarationInfo dec in declarations)
				ValidateDependenciesExist(dec, decList);

			// Sort the Values by type.
			DeclarationInfo[] sorted = SortDeclarations(declarations);

			DocumentationHelper.WriteRegionStart(wr, "Constants & Types", indentOffset);
			for (int i = 0; i < sorted.Length; i++)
			{
				sorted[i].Write(wr, indentOffset);
				if (i != sorted.Length - 1)
					DocumentationHelper.WriteLine(wr); // Leave a line between declarations.
			}
			DocumentationHelper.WriteRegionEnd(wr, "Constants & Types", indentOffset);
		}

		#endregion Methods
	}
}
