//********************************************************************************************************************************
// Filename:    FileInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified VHDL file.
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
	///   Represents a VHDL file.
	/// </summary>
	public class FileInfo
	{
		#region Properties

		/// <summary>
		///   Description of the file.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		///   Name of the file.
		/// </summary>
		public string FileName { get; private set; }

		/// <summary>
		///   Extension added to the filename.
		/// </summary>
		public string FileNameExtension { get; private set; }

		/// <summary>
		///   <see cref="Module"/> that this file represents.
		/// </summary>
		public ModuleInfo Module { get; private set; }

		/// <summary>
		///   RelativePath of the folder where the file represented.
		/// </summary>
		public string RelativePath { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="FileInfo"/> object.
		/// </summary>
		/// <param name="info"><see cref="ModuleInfo"/> object containing the VHDL code to place in the file.</param>
		/// <param name="relativePath">Relative path where the file is represented.</param>
		/// <param name="description">Description of the file.</param>
		/// <param name="fileNameExtension">Extension to add to the filename. (Ex: 'designer' would be for filename.designer.cs). Can be null or empty.</param>
		/// <exception cref="ArgumentNullException"><paramref name="nameSpace"/> or <paramref name="info"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="nameSpace"/> is an empty string.</exception>
		/// <exception cref="ArgumentException"><paramref name="relativePath"/> is defined, but is not a relative path.</exception>
		public FileInfo(ModuleInfo info, string relativePath = null, string description = null, string fileNameExtension = null)
		{
			if (info == null)
				throw new ArgumentNullException("info");

			if (relativePath == null)
				relativePath = string.Empty;

			if (relativePath.Length > 0)
				if (Path.IsPathRooted(relativePath))
					throw new ArgumentException("relativePath is rooted. It must be a relative path.");

			if (fileNameExtension == null || fileNameExtension.Length == 0)
				fileNameExtension = string.Empty;
			else
				fileNameExtension = string.Format(".{0}", fileNameExtension);
			FileName = string.Format("{0}{1}.vhdl", info.Entity.Name, fileNameExtension);
			RelativePath = relativePath;
			Module = info;
			Description = description;
			FileNameExtension = fileNameExtension;
		}

		/// <summary>
		///   Generates the generate section sub-information.
		/// </summary>
		/// <param name="info"><see cref="GenerateInfo"/> object to generate the sub-information on.</param>
		/// <returns>Lookup table of the sub-information.</returns>
		private Dictionary<string, List<string>> GenerateGeneratesSubInfo(GenerateInfo info)
		{
			Dictionary<string, List<string>> lookup = new Dictionary<string, List<string>>();

			// Add Processes.
			if (info.Processes.Count > 0)
				lookup.Add("Processes:", GenerateNamesFromBaseTypeInfo(info.Processes.ToArray()));

			// Add Generates.
			if (info.Generates.Count > 0)
				lookup.Add("Generates:", GenerateNamesFromBaseTypeInfo(info.Generates.ToArray()));

			// Add Sub-Modules.
			if (info.SubModules.Count > 0)
				lookup.Add("Sub-Modules:", GenerateNamesFromBaseTypeInfo(info.SubModules.ToArray()));

			return lookup;
		}

		/// <summary>
		///   This method generates the module information for the file's sub-header, including the sub-information (child functions, signals, etc.).
		/// </summary>
		/// <param name="info"><see cref="ModuleInfo"/> object to generate the information on.</param>
		/// <param name="rootInfo">Root information lookup table to add this module to.</param>
		/// <param name="subInfo">Sub-information lookup table to add this module's sub-information to.</param>
		private void GenerateModuleInfo(ModuleInfo info, out List<string> rootInfo, out List<Dictionary<string, List<string>>> subInfo)
		{
			subInfo = new List<Dictionary<string, List<string>>>();
			rootInfo = new List<string>();

			// Add the actual module first.
			rootInfo.Add(string.Format("{0} (entity)", info.Entity.Name));
			subInfo.Add(GenerateModuleSubInfo(info));

			// Add child generates next.
			if (info.Generates.Count > 0)
			{
				foreach (GenerateInfo child in info.Generates)
				{
					if(child.Generates.Count != 0 || child.Processes.Count != 0 || child.SubModules.Count != 0)
					{
						rootInfo.Add(string.Format("{0} (generate)", child.Name));
						subInfo.Add(GenerateGeneratesSubInfo(child));
					}
				}
			}
		}

		/// <summary>
		///   Generates the module sub-information.
		/// </summary>
		/// <param name="info"><see cref="ModuleInfo"/> object to generate the sub-information on.</param>
		/// <returns>Lookup table of the sub-information.</returns>
		private Dictionary<string, List<string>> GenerateModuleSubInfo(ModuleInfo info)
		{
			Dictionary<string, List<string>> lookup = new Dictionary<string, List<string>>();

			// Add Functions.
			if (info.Functions.Count > 0)
				lookup.Add("Functions:", GenerateNamesFromBaseTypeInfo(info.Functions.ToArray()));

			// Add Declarations.
			if (info.DeclaredTypes.Count > 0)
				lookup.Add("Constants & Types:", GenerateNamesFromBaseTypeInfo(info.DeclaredTypes.ToArray()));

			// Add Procedures.
			if (info.Procedures.Count > 0)
				lookup.Add("Procedures:", GenerateNamesFromBaseTypeInfo(info.Procedures.ToArray()));

			// Add Components.
			ComponentInfo[] components = SubModule.GetUniqueComponents(info.SubModules);
			if (components.Length > 0)
				lookup.Add("Components:", GenerateNamesFromBaseTypeInfo(components));

			// Add Signals.
			if (info.Signals.Count > 0)
				lookup.Add("Signals:", GenerateNamesFromBaseTypeInfo(info.Signals.ToArray()));

			// Add Aliases.
			if (info.Aliases.Count > 0)
				lookup.Add("Aliases:", GenerateNamesFromBaseTypeInfo(info.Aliases.ToArray()));

			// Add Attributes.
			AttributeDeclarationInfo[] declarations = AttributeDeclarationInfo.GetUniqueAttributeDeclarations(info.Attributes);
			if (declarations.Length > 0)
				lookup.Add("Attributes:", GenerateNamesFromBaseTypeInfo(declarations));

			// Add Processes.
			if (info.Processes.Count > 0)
				lookup.Add("Processes:", GenerateNamesFromBaseTypeInfo(info.Processes.ToArray()));

			// Add Generates.
			if (info.Generates.Count > 0)
				lookup.Add("Generates:", GenerateNamesFromBaseTypeInfo(info.Generates.ToArray()));

			// Add Sub-Modules.
			if (info.SubModules.Count > 0)
				lookup.Add("Sub-Modules:", GenerateNamesFromBaseTypeInfo(info.SubModules.ToArray()));

			return lookup;
		}

		/// <summary>
		///   Generates a list of names from the array of <see cref="BaseTypeInfo"/> objects.
		/// </summary>
		/// <param name="types">Array of <see cref="BaseTypeInfo"/> objects</param>
		/// <returns><see cref="List{T}"/> of strings representing the names of the <see cref="BaseTypeInfo"/> objects.</returns>
		private List<string> GenerateNamesFromBaseTypeInfo(BaseTypeInfo[] types)
		{
			List<string> returnList = new List<string>(types.Length);
			foreach (BaseTypeInfo type in types)
				returnList.Add(type.Name);
			return returnList;
		}

		/// <summary>
		///   Determines the maximum string length from the previous max and a text string.
		/// </summary>
		/// <param name="text">Text to evaluate if larger.</param>
		/// <param name="previousMax">Previous maximum length.</param>
		private void MaxStringLength(string text, ref int previousMax)
		{
			if (text.Length > previousMax)
				previousMax = text.Length;
		}

		/// <summary>
		///   Generates the library lookup table.
		/// </summary>
		/// <returns>Lookup table for the library and use statements.</returns>
		private Dictionary<string, List<string>> GetLibraryLookup()
		{
			Dictionary<string, List<string>> lookup = new Dictionary<string, List<string>>();
			string[] usings = Module.Usings;
			foreach (string use in usings)
			{
				string[] splits = use.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
				if (!lookup.ContainsKey(splits[0]))
					lookup.Add(splits[0], new List<string>());
				lookup[splits[0]].Add(use);
			}
			return lookup;
		}

		/// <summary>
		///   Writes the file to the <see cref="StreamWriter"/> object.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the code to.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		private void WriteFileSubHeader(StreamWriter wr)
		{
			List<Dictionary<string, List<string>>> subInfo = null;
			List<string> info = null;
			GenerateModuleInfo(Module, out info, out subInfo);

			// Determine the first part maximum string length.
			int maxSizeRegions = 0;
			foreach (Dictionary<string, List<string>> lookup in subInfo)
			{
				foreach (string key in lookup.Keys)
					MaxStringLength(key, ref maxSizeRegions);
			}

			int col1 = maxSizeRegions + 6; // 6 is for "--   " and space after.
			int index = 0;
			foreach (string key in info)
			{
				// Write the module of the file.
				StringBuilder sb = new StringBuilder();
				sb.Append("-- ");
				sb.Append(key);
				DocumentationHelper.WriteLine(wr, sb.ToString(), 0);
				Dictionary<string, List<string>> lookup = subInfo[index++];
				int subIndex = 0;
				foreach (string subKey in lookup.Keys)
				{
					DocumentationHelper.WriteLine(wr, string.Format("--   {0}", subKey), 0);
					for (int i = 0; i < lookup[subKey].Count; i++)
					{
						sb.Clear();
						sb.Append("--");
						for (int j = 2; j < col1; j++)
							sb.Append(" ");
						sb.Append(lookup[subKey][i]);
						DocumentationHelper.WriteLine(wr, sb.ToString(), 0);
					}
					subIndex++;

					if (subIndex != lookup.Keys.Count)
						DocumentationHelper.WriteLine(wr, "--", 0);
				}
				DocumentationHelper.WriteFlowerLine(wr, 0);
			}
		}

		/// <summary>
		///   Writes the source code information in this object out to a file in the form of source code.
		/// </summary>
		/// <param name="rootFolder">Root location of the file. (The relative path will be added to this folder to generate the file.)</param>
		/// <exception cref="ArgumentNullException"><paramref name="rootFolder"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="rootFolder"/> is not a valid folder path.</exception>
		/// <exception cref="IOException">An error occurred while writing to the file.</exception>
		public void WriteToFile(string rootFolder)
		{
			if (rootFolder == null)
				throw new ArgumentNullException("rootFolder");
			if (rootFolder.Length == 0)
				throw new ArgumentException("rootFolder is an empty string");
			try
			{
				rootFolder = Path.GetFullPath(rootFolder);
			}
			catch (Exception e)
			{
				throw new ArgumentException("rootFolder is not a valid path (see inner exception).", e);
			}

			string fullFolderPath;
			if (RelativePath.Length > 0)
				fullFolderPath = Path.Combine(rootFolder, RelativePath);
			else
				fullFolderPath = rootFolder;
			string fullPath = Path.Combine(fullFolderPath, FileName);

			// Generate any needed directories.
			DefaultValues.CreateFolderPath(fullFolderPath);
			using (StreamWriter wr = new StreamWriter(fullPath))
			{
				DocumentationHelper.WriteFileHeader(wr, FileName, Description);

				if (DefaultValues.IncludeSubHeader)
					WriteFileSubHeader(wr);

				// Add usings.
				string[] usings = Module.Usings;
				if (usings.Length > 0)
				{
					Dictionary<string, List<string>> lookup = GetLibraryLookup();
					foreach(string key in lookup.Keys)
					{
						DocumentationHelper.WriteLine(wr, string.Format("library {0};", key), 0);
						foreach (string use in lookup[key])
							DocumentationHelper.WriteLine(wr, string.Format("use {0};", use), 0);
						DocumentationHelper.WriteLine(wr);
					}
				}

				Module.Write(wr, 0);
			}
		}

		#endregion Methods
	}
}
