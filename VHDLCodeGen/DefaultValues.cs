//********************************************************************************************************************************
// Filename:    DefaultValues.cs
// Owner:       Richard Dunkley
// Description: Static class containing the default values of configuration settings of the code generation library. Consuming
//              applications should set the static properties so the auto-generated code is documented appropriately.
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
using System.Reflection;

namespace VHDLCodeGen
{
	/// <summary>
	///   Provides default values to the code in this assembly.
	/// </summary>
	public static class DefaultValues
	{
		#region Properties

		/// <summary>
		///   Name of the application using this assembly.
		/// </summary>
		public static string ApplicationName { get; private set; }

		/// <summary>
		///   Version of the application using this assembly.
		/// </summary>
		public static string ApplicationVersion { get; private set; }

		/// <summary>
		///   Name of the Company that will be in the generated code.
		/// </summary>
		public static string CompanyName { get; set; }

		/// <summary>
		///   Template for the copyright statement.
		/// </summary>
		/// <remarks>This line is wrapped if needed.</remarks>
		public static string CopyrightTemplate { get; set; }

		/// <summary>
		///   Name of the developer that will be in the generated code.
		/// </summary>
		public static string Developer { get; set; }

		/// <summary>
		///   Template for the file information header.
		/// </summary>
		/// <remarks>
		///   These lines should not exceed <see cref="NumCharactersPerLine"/> since the lines will not be wrapped.
		/// </remarks>
		public static string[] FileInfoTemplate { get; set; }

		/// <summary>
		///   Flower box character to use for documentation. Can be null.
		/// </summary>
		public static char? FlowerBoxCharacter { get; set; }

		/// <summary>
		///   Determines whether the sub-header should be added to the file header.
		/// </summary>
		/// <remarks>The sub-header breaks down all the components of the class.</remarks>
		public static bool IncludeSubHeader { get; set; }

		/// <summary>
		///   Name of this assembly.
		/// </summary>
		public static string LibraryName { get; private set; }

		/// <summary>
		///   Version of this assembly.
		/// </summary>
		public static string LibraryVersion { get; private set; }

		/// <summary>
		///   Template for the auto-generated code license.
		/// </summary>
		/// <remarks>This line is wrapped if needed.</remarks>
		public static string[] LicenseTemplate { get; set; }

		/// <summary>
		///   Number of characters allowed per line. This only restricts documentation lines.
		/// </summary>
		public static int NumCharactersPerLine { get; set; }

		/// <summary>
		///   Number of spaces per tab.
		/// </summary>
		public static int TabSize { get; set; }

		/// <summary>
		///   True if tabs should be used for indentation, false if spaces should be used.
		/// </summary>
		public static bool UseTabs { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Generates the default values.
		/// </summary>
		static DefaultValues()
		{
			Assembly entry = Assembly.GetEntryAssembly();
			if (entry == null)
			{
				ApplicationName = string.Empty;
				ApplicationVersion = string.Empty;
			}
			else
			{
				ApplicationName = entry.GetName().Name;
				ApplicationVersion = entry.GetName().Version.ToString(3);
			}

			AssemblyName library = Assembly.GetExecutingAssembly().GetName();
			LibraryName = library.Name;
			LibraryVersion = library.Version.ToString(3);

			CompanyName = "Specify your company name here by setting CSCodeGen.DefaultValues.CompanyName property";
			Developer = "Specify your developer name here by setting CSCodeGen.DefaultValues.Developer property";
			FlowerBoxCharacter = '-';
			TabSize = 4;
			UseTabs = true;
			NumCharactersPerLine = 130;
			FileInfoTemplate = GenerateFileInfoTemplate();
			CopyrightTemplate = GenerateCopyrightTemplate();
			LicenseTemplate = GenerateLicenseTemplate();
			IncludeSubHeader = true;
		}

		/// <summary>
		///   Creates the path up to the folder specified.
		/// </summary>
		/// <param name="folder"></param>
		/// <returns>Returns the full path to the folder.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="folder"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="folder"/> is not a valid folder.</exception>
		public static string CreateFolderPath(string folder)
		{
			if (folder == null)
				throw new ArgumentNullException("folder");

			try
			{
				folder = Path.GetFullPath(folder);
			}
			catch (Exception e)
			{
				throw new ArgumentException("The folder provided is not a valid folder (See inner exception).", e);
			}

			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);
			return folder;
		}

		/// <summary>
		///   Generates a default Copyright template.
		/// </summary>
		/// <returns>Array of the lines in the template.</returns>
		private static string GenerateCopyrightTemplate()
		{
			return "Copyright © <%developer%> <%year%>";
		}

		/// <summary>
		///   Generates a default file information template.
		/// </summary>
		/// <returns>Array of the lines in the template.</returns>
		private static string[] GenerateFileInfoTemplate()
		{
			List<string> template = new List<string>();
			template.Add("-- Filename:    <%filename%>");
			template.Add("-- Owner:       <%developer%>");
			template.Add("-- Description: <%description%>");
			template.Add("-- Generated using <%appname%> version <%appversion%> with <%libraryname%>.dll version <%libraryversion%>.");
			return template.ToArray();
		}

		/// <summary>
		///   Generates a default license template.
		/// </summary>
		/// <returns>Array of the lines in the template.</returns>
		private static string[] GenerateLicenseTemplate()
		{
			List<string> template = new List<string>();
			template.Add("You can add a license of your choice here by setting VHDLCodeGen.DefaultValues.License property.");
			template.Add("Here is a list of template items that may aid in your license, file header, or copyright template:");
			template.Add("");
			template.Add("	<%year%> - Replaced with the current year.");
			template.Add("	<%date%> - Replaced with the current date.");
			template.Add("	<%time%> - Replaced with the current time.");
			template.Add("	<%datetime%> - Replaced with the current date and time.");
			template.Add("	<%developer%> - Replaced with the Developer's Name (as specified in the DefaultValues properties).");
			template.Add("	<%company%> - Replaced with the Company Name (as specified in the DefaultValues properties).");
			template.Add("	<%appversion%> - Replaced with the calling application's version.");
			template.Add("	<%appname%> - Replaced with the calling application's name.");
			template.Add("	<%libraryversion%> - Replaced with the CSCodeGen library version.");
			template.Add("	<%libraryname%> - Replaced with the CSCodeGen library name.");
			template.Add("	<%copyright%> - Replaced with the copyright statement (cannot be cyclic).");
			template.Add("	<%license%> - Replaced with the license statement (cannot be cyclic).");
			template.Add("	<%filename%> - Replaced with the file name being generated.");
			template.Add("	<%description%> - Replaced with a description of the file being generated.");
			return template.ToArray();
		}

		#endregion Methods
	}
}
