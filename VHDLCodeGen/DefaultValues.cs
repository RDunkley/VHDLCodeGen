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
		/// <remarks>
		///   No leading comments are required. This line is wrapped if needed. The following tags can be used in the copyright template.
		///   <list type="bullet">
		///     <item>
		///       <term>&lt;%year%&gt;</code></term>
		///       <description>Replaced with the current year.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%date%&gt;</code></term>
		///       <description>Replaced with the current date.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%time%&gt;</code></term>
		///       <description>Replaced with the current time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%datetime%&gt;</code></term>
		///       <description>Replaced with the current date and time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%developer%&gt;</code></term>
		///       <description>Replaced with the Developer's Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%company%&gt;</code></term>
		///       <description>Replaced with the Company Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appversion%&gt;</code></term>
		///       <description>Replaced with the calling application's version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appname%&gt;</code></term>
		///       <description>Replaced with the calling application's name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryversion%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryname%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%copyright%&gt;</code></term>
		///       <description>Replaced with the copyright statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%license%&gt;</code></term>
		///       <description>Replaced with the license statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%filename%&gt;</code></term>
		///       <description>Replaced with the file name being generated.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%description%&gt;</code></term>
		///       <description>Replaced with a description of the file being generated.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%flowerfill%&gt;</code></term>
		///       <description>Fills the rest of the line with the flower box character. Should be specified at end of line unless automatic wrapping occurs.</description>
		///     </item>
		///   </list>
		/// </remarks>
		public static string CopyrightTemplate { get; set; }

		/// <summary>
		///   Name of the developer that will be in the generated code.
		/// </summary>
		public static string Developer { get; set; }

		/// <summary>
		///   Template for the file information header.
		/// </summary>
		/// <remarks>
		///   These lines should not exceed <see cref="NumCharactersPerLine"/> since the lines will not be wrapped. The starting comment lines should 
		///   be added to the template since this template is dropped directly in as the header after tag conversion. The following tags can be used in
		///   the file information template.
		///   <list type="bullet">
		///     <item>
		///       <term>&lt;%year%&gt;</code></term>
		///       <description>Replaced with the current year.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%date%&gt;</code></term>
		///       <description>Replaced with the current date.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%time%&gt;</code></term>
		///       <description>Replaced with the current time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%datetime%&gt;</code></term>
		///       <description>Replaced with the current date and time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%developer%&gt;</code></term>
		///       <description>Replaced with the Developer's Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%company%&gt;</code></term>
		///       <description>Replaced with the Company Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appversion%&gt;</code></term>
		///       <description>Replaced with the calling application's version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appname%&gt;</code></term>
		///       <description>Replaced with the calling application's name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryversion%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryname%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%copyright%&gt;</code></term>
		///       <description>Replaced with the copyright statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%license%&gt;</code></term>
		///       <description>Replaced with the license statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%filename%&gt;</code></term>
		///       <description>Replaced with the file name being generated.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%description%&gt;</code></term>
		///       <description>Replaced with a description of the file being generated.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%flowerfill%&gt;</code></term>
		///       <description>Fills the rest of the line with the flower box character. Should be specified at end of line unless automatic wrapping occurs.</description>
		///     </item>
		///   </list>
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
		/// <remarks>
		///   No leading comments are required. This line is wrapped if needed. The following tags can be used in the license template.
		///   <list type="bullet">
		///     <item>
		///       <term>&lt;%year%&gt;</code></term>
		///       <description>Replaced with the current year.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%date%&gt;</code></term>
		///       <description>Replaced with the current date.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%time%&gt;</code></term>
		///       <description>Replaced with the current time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%datetime%&gt;</code></term>
		///       <description>Replaced with the current date and time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%developer%&gt;</code></term>
		///       <description>Replaced with the Developer's Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%company%&gt;</code></term>
		///       <description>Replaced with the Company Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appversion%&gt;</code></term>
		///       <description>Replaced with the calling application's version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appname%&gt;</code></term>
		///       <description>Replaced with the calling application's name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryversion%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryname%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%copyright%&gt;</code></term>
		///       <description>Replaced with the copyright statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%license%&gt;</code></term>
		///       <description>Replaced with the license statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%filename%&gt;</code></term>
		///       <description>Replaced with the file name being generated.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%description%&gt;</code></term>
		///       <description>Replaced with a description of the file being generated.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%flowerfill%&gt;</code></term>
		///       <description>Fills the rest of the line with the flower box character. Should be specified at end of line unless automatic wrapping occurs.</description>
		///     </item>
		///   </list>
		/// </remarks>
		public static string[] LicenseTemplate { get; set; }

		/// <summary>
		///   Number of characters allowed per line. This only restricts documentation lines.
		/// </summary>
		public static int NumCharactersPerLine { get; set; }

		/// <summary>
		///   Template for the section start identifier.
		/// </summary>
		/// <remarks>
		///   These lines should be written for variable length lines since they may start indented (use flower fill). The starting comment on
		///   each line should be added as well since these lines are used directly after tag conversion. The following tags can be
		///   used in the section identifier template:
		///   <list type="bullet">
		///     <item>
		///       <term>&lt;%year%&gt;</code></term>
		///       <description>Replaced with the current year.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%date%&gt;</code></term>
		///       <description>Replaced with the current date.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%time%&gt;</code></term>
		///       <description>Replaced with the current time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%datetime%&gt;</code></term>
		///       <description>Replaced with the current date and time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%developer%&gt;</code></term>
		///       <description>Replaced with the Developer's Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%company%&gt;</code></term>
		///       <description>Replaced with the Company Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appversion%&gt;</code></term>
		///       <description>Replaced with the calling application's version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appname%&gt;</code></term>
		///       <description>Replaced with the calling application's name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryversion%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryname%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%copyright%&gt;</code></term>
		///       <description>Replaced with the copyright statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%license%&gt;</code></term>
		///       <description>Replaced with the license statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%param%&gt;</code></term>
		///       <description>Replaced with the name of the section.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%flowerfill%&gt;</code></term>
		///       <description>Fills the rest of the line with the flower box character. Should be specified at end of line unless automatic wrapping occurs.</description>
		///     </item>
		///   </list>
		/// </remarks>
		public static string[] SectionStartTemplate { get; set; }

		/// <summary>
		///   Template for the section stop identifier.
		/// </summary>
		/// <remarks>
		///   These lines should be written for variable length lines since they may start indented (use flower fill). The starting comment on
		///   each line should be added as well since these lines are used directly after tag conversion. The following tags can be
		///   used in the section identifier template:
		///   <list type="bullet">
		///     <item>
		///       <term>&lt;%year%&gt;</code></term>
		///       <description>Replaced with the current year.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%date%&gt;</code></term>
		///       <description>Replaced with the current date.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%time%&gt;</code></term>
		///       <description>Replaced with the current time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%datetime%&gt;</code></term>
		///       <description>Replaced with the current date and time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%developer%&gt;</code></term>
		///       <description>Replaced with the Developer's Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%company%&gt;</code></term>
		///       <description>Replaced with the Company Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appversion%&gt;</code></term>
		///       <description>Replaced with the calling application's version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appname%&gt;</code></term>
		///       <description>Replaced with the calling application's name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryversion%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryname%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%copyright%&gt;</code></term>
		///       <description>Replaced with the copyright statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%license%&gt;</code></term>
		///       <description>Replaced with the license statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%param%&gt;</code></term>
		///       <description>Replaced with the name of the section.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%flowerfill%&gt;</code></term>
		///       <description>Fills the rest of the line with the flower box character. Should be specified at end of line unless automatic wrapping occurs.</description>
		///     </item>
		///   </list>
		/// </remarks>
		public static string[] SectionEndTemplate { get; set; }

		/// <summary>
		///   Number of spaces per tab.
		/// </summary>
		public static int TabSize { get; set; }

		/// <summary>
		///   True if tabs should be used for indentation, false if spaces should be used.
		/// </summary>
		public static bool UseTabs { get; set; }

		/// <summary>
		///   True if the optional type names should be added at the end of type declarations.
		/// </summary>
		/// <remarks>
		///   The VHDL specification allows for optional type names on the following items: entity, architecture, function,
		///   procedure, and package. These optional type names are placed after the end statement. For example, the end
		///   of an entity can be 'end;' or 'end entity;'.
		/// </remarks>
		public static bool AddOptionalTypeNames { get; set; }

		/// <summary>
		///   True if the optional type names should be added at the end of type declarations.
		/// </summary>
		/// <remarks>
		///   The VHDL specification allows for optional names/labels on the following items: entity, architecture, component,
		///   function, procedure, generate, process, record, and package. These optional name/labels are placed after the end
		///   statement and type statement. For example, the end of an entity can be 'end entity;' or 'end entity TEST_MODULE;'.
		/// </remarks>
		public static bool AddOptionalNames { get; set; }

		/// <summary>
		///   True if a space should be added after keywords, false otherwise.
		/// </summary>
		/// <remarks>
		///   For example, a process declaraction can be written as 'process (' or 'process('. This boolean value determines which is chosen.
		///   This applies to ports, generics, functions, procedures, and processes.
		/// </remarks>
		public static bool AddSpaceAfterKeyWords { get; set; }

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
			SectionStartTemplate = GenerateSectionStartTemplate();
			SectionEndTemplate = GenerateSectionEndTemplate();
			IncludeSubHeader = true;
			AddOptionalTypeNames = true;
			AddOptionalNames = true;
			AddSpaceAfterKeyWords = false;
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
		/// <remarks>
		///   The following tags can be used in the copyright template.
		///   <list type="bullet">
		///     <item>
		///       <term>&lt;%year%&gt;</code></term>
		///       <description>Replaced with the current year.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%date%&gt;</code></term>
		///       <description>Replaced with the current date.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%time%&gt;</code></term>
		///       <description>Replaced with the current time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%datetime%&gt;</code></term>
		///       <description>Replaced with the current date and time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%developer%&gt;</code></term>
		///       <description>Replaced with the Developer's Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%company%&gt;</code></term>
		///       <description>Replaced with the Company Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appversion%&gt;</code></term>
		///       <description>Replaced with the calling application's version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appname%&gt;</code></term>
		///       <description>Replaced with the calling application's name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryversion%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryname%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%copyright%&gt;</code></term>
		///       <description>Replaced with the copyright statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%license%&gt;</code></term>
		///       <description>Replaced with the license statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%filename%&gt;</code></term>
		///       <description>Replaced with the file name being generated.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%description%&gt;</code></term>
		///       <description>Replaced with a description of the file being generated.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%flowerfill%&gt;</code></term>
		///       <description>Fills the rest of the line with the flower box character. Should be specified at end of line unless automatic wrapping occurs.</description>
		///     </item>
		///   </list>
		/// </remarks>
		private static string GenerateCopyrightTemplate()
		{
			return "Copyright © <%developer%> <%year%>";
		}

		/// <summary>
		///   Generates a default file information template.
		/// </summary>
		/// <returns>Array of the lines in the template.</returns>
		/// <remarks>
		///   The following tags can be used in the file information template.
		///   <list type="bullet">
		///     <item>
		///       <term>&lt;%year%&gt;</code></term>
		///       <description>Replaced with the current year.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%date%&gt;</code></term>
		///       <description>Replaced with the current date.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%time%&gt;</code></term>
		///       <description>Replaced with the current time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%datetime%&gt;</code></term>
		///       <description>Replaced with the current date and time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%developer%&gt;</code></term>
		///       <description>Replaced with the Developer's Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%company%&gt;</code></term>
		///       <description>Replaced with the Company Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appversion%&gt;</code></term>
		///       <description>Replaced with the calling application's version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appname%&gt;</code></term>
		///       <description>Replaced with the calling application's name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryversion%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryname%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%copyright%&gt;</code></term>
		///       <description>Replaced with the copyright statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%license%&gt;</code></term>
		///       <description>Replaced with the license statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%filename%&gt;</code></term>
		///       <description>Replaced with the file name being generated.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%description%&gt;</code></term>
		///       <description>Replaced with a description of the file being generated.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%flowerfill%&gt;</code></term>
		///       <description>Fills the rest of the line with the flower box character. Should be specified at end of line unless automatic wrapping occurs.</description>
		///     </item>
		///   </list>
		/// </remarks>
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
		/// <remarks>
		///   The following tags can be used in the license template.
		///   <list type="bullet">
		///     <item>
		///       <term>&lt;%year%&gt;</code></term>
		///       <description>Replaced with the current year.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%date%&gt;</code></term>
		///       <description>Replaced with the current date.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%time%&gt;</code></term>
		///       <description>Replaced with the current time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%datetime%&gt;</code></term>
		///       <description>Replaced with the current date and time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%developer%&gt;</code></term>
		///       <description>Replaced with the Developer's Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%company%&gt;</code></term>
		///       <description>Replaced with the Company Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appversion%&gt;</code></term>
		///       <description>Replaced with the calling application's version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appname%&gt;</code></term>
		///       <description>Replaced with the calling application's name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryversion%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryname%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%copyright%&gt;</code></term>
		///       <description>Replaced with the copyright statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%license%&gt;</code></term>
		///       <description>Replaced with the license statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%filename%&gt;</code></term>
		///       <description>Replaced with the file name being generated.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%description%&gt;</code></term>
		///       <description>Replaced with a description of the file being generated.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%flowerfill%&gt;</code></term>
		///       <description>Fills the rest of the line with the flower box character. Should be specified at end of line unless automatic wrapping occurs.</description>
		///     </item>
		///   </list>
		/// </remarks>
		private static string[] GenerateLicenseTemplate()
		{
			List<string> template = new List<string>();
			template.Add("You can add a license of your choice here by setting VHDLCodeGen.DefaultValues.License property.");
			//template.Add("Here is a list of template items that may aid in your license, file header, or copyright template:");
			//template.Add("");
			//template.Add("	<%year%> - Replaced with the current year.");
			//template.Add("	<%date%> - Replaced with the current date.");
			//template.Add("	<%time%> - Replaced with the current time.");
			//template.Add("	<%datetime%> - Replaced with the current date and time.");
			//template.Add("	<%developer%> - Replaced with the Developer's Name (as specified in the DefaultValues properties).");
			//template.Add("	<%company%> - Replaced with the Company Name (as specified in the DefaultValues properties).");
			//template.Add("	<%appversion%> - Replaced with the calling application's version.");
			//template.Add("	<%appname%> - Replaced with the calling application's name.");
			//template.Add("	<%libraryversion%> - Replaced with the CSCodeGen library version.");
			//template.Add("	<%libraryname%> - Replaced with the CSCodeGen library name.");
			//template.Add("	<%copyright%> - Replaced with the copyright statement (cannot be cyclic).");
			//template.Add("	<%license%> - Replaced with the license statement (cannot be cyclic).");
			//template.Add("	<%filename%> - Replaced with the file name being generated.");
			//template.Add("	<%description%> - Replaced with a description of the file being generated.");
			return template.ToArray();
		}

		/// <summary>
		///   Generates a default section start header.
		/// </summary>
		/// <returns>Array of the lines in the template.</returns>
		/// <remarks>
		///   The following tags can be used in the license template.
		///   <list type="bullet">
		///     <item>
		///       <term>&lt;%year%&gt;</code></term>
		///       <description>Replaced with the current year.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%date%&gt;</code></term>
		///       <description>Replaced with the current date.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%time%&gt;</code></term>
		///       <description>Replaced with the current time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%datetime%&gt;</code></term>
		///       <description>Replaced with the current date and time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%developer%&gt;</code></term>
		///       <description>Replaced with the Developer's Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%company%&gt;</code></term>
		///       <description>Replaced with the Company Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appversion%&gt;</code></term>
		///       <description>Replaced with the calling application's version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appname%&gt;</code></term>
		///       <description>Replaced with the calling application's name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryversion%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryname%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%copyright%&gt;</code></term>
		///       <description>Replaced with the copyright statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%license%&gt;</code></term>
		///       <description>Replaced with the license statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%param%&gt;</code></term>
		///       <description>Replaced with the name of the section.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%flowerfill%&gt;</code></term>
		///       <description>Fills the rest of the line with the flower box character. Should be specified at end of line unless automatic wrapping occurs.</description>
		///     </item>
		///   </list>
		/// </remarks>
		private static string[] GenerateSectionStartTemplate()
		{
			List<string> template = new List<string>();
			template.Add("------------------------------------------------ <%param%> <%flowerfill%>");
			return template.ToArray();
		}

		/// <summary>
		///   Generates a default section start header.
		/// </summary>
		/// <returns>Array of the lines in the template.</returns>
		/// <remarks>
		///   The following tags can be used in the license template.
		///   <list type="bullet">
		///     <item>
		///       <term>&lt;%year%&gt;</code></term>
		///       <description>Replaced with the current year.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%date%&gt;</code></term>
		///       <description>Replaced with the current date.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%time%&gt;</code></term>
		///       <description>Replaced with the current time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%datetime%&gt;</code></term>
		///       <description>Replaced with the current date and time.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%developer%&gt;</code></term>
		///       <description>Replaced with the Developer's Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%company%&gt;</code></term>
		///       <description>Replaced with the Company Name (as specified in the DefaultValues properties).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appversion%&gt;</code></term>
		///       <description>Replaced with the calling application's version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%appname%&gt;</code></term>
		///       <description>Replaced with the calling application's name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryversion%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library version.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%libraryname%&gt;</code></term>
		///       <description>Replaced with the CSCodeGen library name.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%copyright%&gt;</code></term>
		///       <description>Replaced with the copyright statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%license%&gt;</code></term>
		///       <description>Replaced with the license statement (cannot be cyclic).</description>
		///     </item>
		///     <item>
		///       <term>&lt;%param%&gt;</code></term>
		///       <description>Replaced with the name of the section.</description>
		///     </item>
		///     <item>
		///       <term>&lt;%flowerfill%&gt;</code></term>
		///       <description>Fills the rest of the line with the flower box character. Should be specified at end of line unless automatic wrapping occurs.</description>
		///     </item>
		///   </list>
		/// </remarks>
		private static string[] GenerateSectionEndTemplate()
		{
			return null; // No end section specified.
		}

		#endregion Methods
	}
}
