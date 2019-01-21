# VHDLCodeGen
VHDL Code Generation Library - C# library which allows consumer to easily generate simplified well-structured VHDL code. The classes represent different components of the VHDL language. An application can use the data structures to create components in a VHDL file and then rely on the library to organize and document the components and generate the actual files.

This library is not intended to represent all the possible code structures or implementation of VHDL. It is meant to provide a simplified view of creating VHDL modules so that a code generation tool can very quickly and easily generate well-structured, simplified code.

Well-structured referenced above means that the code is documented and organized according to strict guidelines. The software classes require minimum documentation text to be added when the objects are created and generates the documentation based on this text. It currently provides the following structured features:

1. Adds flower boxes around documentation. Adds flower box lines before and after all documentation. The flower box character can be specified using DefaultValues.FlowerBoxCharacter. It can be set to null, to omit the flower box lines.

2. Generates a file sub-header. Some coding standards require a sub-header at the top of the file that lists the components of the file for a quick reference. This can be turned on or off using DefaultValues.IncludeSubHeader.

3. File header template. The library allows you to specify a file header in template form so that it can populate it and add it to all the code files generated. You specify the template using DefaultValues.FileInfoTemplate. Note: the string lines provided will be added to the top of the file directly.

4. Copyright template. The library allows you to specify a copyright statement in template form so that it can populate it and add it to all the code files generated. This allows you to specify a simple copyright (Ex: "Copyright © <%developer%> <%year%>") or a much more complex one. The copyright template is specified using DefaultValues.CopyrightTemplate. Note: the string line is added to comment line(s) directly below the file header so no need to add the required comment characters at the first of it.

5. License template. The library allows you to specify a license statement in template form so that it can populate it and add it to all the code files generated. If you are planning on open-sourcing the code you generate you won't need to add the information later. The license template can be set using DefaultValues.LicenseTemplate. These lines are added after the appropriate comment characters so you can copy this strait out of your typical license file.

6. Can specify the number of spaces in the indentation for the files. This is set using DefaultValues.TabSize.

7. Can specify whether tabs or spaces should be used for indentation. This is set using DefaultValues.UseTabs.

8. Can specify the number of characters per line and will break code and comments to a new line if they exceed the number of characters. This is set using DefaultValues.NumCharactersPerLine.

9. Section Start Template. Allows you to specify the template that will be added to the code at the start of a section. Sections are defined by 'Functions', 'Constants & Types', 'Procedures', 'Components', 'Signals', 'Aliases', 'Attributes', 'Processes', 'Generates', and 'Sub-Modules'. The <%filename%> and <%description%> tags are not supported in this template. The <%param%> tag is the name of the section.

10. Section End Template. Allows you to specify the template that will be added to the code at the end of a section. Sections are defined by 'Functions', 'Constants & Types', 'Procedures', 'Components', 'Signals', 'Aliases', 'Attributes', 'Processes', 'Generates', and 'Sub-Modules'. The <%filename%> and <%description%> tags are not supported in this template. The <%param%> tag is the name of the section.

11. Can specify whether to add the optional type names to the end of entity, architecture, function, procedure, and package. This is set using DefaultValues.AddOptionalTypeNames.

12. Can specify whether to add the optional name/label to the end of entity, architecture, component, function, procedure, and generate. This is set using DefaultValues.AddOptionalNames.

13. Can specify whether to add a space after certain key words in the auto-generated code. For example, he space allows a preference between 'process (' and 'process('. This is set using DefaultValues.AddSpaceAfterKeyWords.


The templates above have defaults in them that you will definitely want to change before generating code. All templates can be left out by providing an empty or null value for the associated template. The templates have the following optional tags that are replaced when the code is generated:

Generic Tags:
	1. <%year%> - Replaced with the current year.

	2. <%date%> - Replaced with the current date.

	3. <%time%> - Replaced with the current time.

	4. <%datetime%> - Replaced with the current date and time.

Global Tags:
	1. <%developer%> - Replaced with the Developer's Name (as specified in the DefaultValues properties).

	2. <%company%> - Replaced with the Company Name (as specified in the DefaultValues properties).

	3. <%appversion%> - Replaced with the calling application's version.

	4. <%appname%> - Replaced with the calling application's name.

	5. <%libraryversion%> - Replaced with the VHDLCodeGen library version.

	6. <%libraryname%> - Replaced with the VHDLCodeGen library name.

	7. <%flowerfill%> - Fills the rest of the line with the flower character.

Other Template Tags:
	1. <%copyright%> - Replaced with the copyright statement (cannot be cyclic).

	2. <%license%> - Replaced with the license statement (cannot be cyclic).

Item Specific Tags:
	1. <%filename%> - Replaced with the file name being generated.

	2. <%description%> - Replaced with a description of the file being generated.

	3. <%param%> - Replaced with a parameter based on context. See individual template for details.
