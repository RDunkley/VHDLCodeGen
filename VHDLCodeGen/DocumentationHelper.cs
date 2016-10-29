//********************************************************************************************************************************
// Filename:    DocumentationHelper.cs
// Owner:       Richard Dunkley
// Description: Static class used to aid in the generation of documentation for VHDL files.
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
	///   Utility class to aid in documentation generation.
	/// </summary>
	public static class DocumentationHelper
	{
		#region Methods

		/// <summary>
		///   Adds a comment string to the end of the text.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <param name="comment">Comment string to be added.</param>
		/// <param name="previousTextSize">Size of any previous text written to the text (beyond the whitespace).</param>
		/// <param name="indentOffset">Number of indentations in the code where the text begins (applies to both the previous written text and newly created lines).</param>
		/// <param name="newLineText">
		///   If the comment is too long then this string is used to specify what text to place before the comment starts up 
		///   again. (Should not include indentation.)
		/// </param>
		/// <remarks>
		///   If the text is longer than <see cref="DefaultValues.NumCharactersPerLine"/> then it will be split to multiple 
		///   lines. If a space could not be found on the string to break it up on then the string will be broken on the 
		///   boundary and a hyphen added. Identations are added to the subsequent lines of the comment prior to the 
		///   <paramref name="newLineText"/> being added. If the indent is so much that the comment cannot be added at all it is added
		///    to the initial text and <see cref="DefaultValues.NumCharactersPerLine"/> is ignored.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		private static void AddCommentToEndOfLine(StreamWriter wr, string comment, int previousTextSize, int indentOffset, string newLineText = null)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			if (comment == null || comment.Length == 0)
				return;

			if (newLineText == null)
				newLineText = string.Empty;

			if (previousTextSize < 0)
				previousTextSize = 0;

			int offset = indentOffset * DefaultValues.TabSize + previousTextSize;
			bool cantfitText = false;
			while (comment != null)
			{
				string lineText, remainingText;
				SplitOnEndOfLine(comment, offset, out lineText, out remainingText);
				if (lineText.Length == 0)
				{
					// Can't fit any text on this text.
					if (cantfitText)
					{
						// This is the second time we can't fit text, which means we can't write any of the text so just write the whole text.
						wr.WriteLine(comment);
						return;
					}
					else
					{
						cantfitText = true;
					}
				}
				else
				{
					// Write the rest of this text and create the new text.
					wr.WriteLine(lineText);
					if (remainingText != null)
					{
						int wsLength;
						wr.Write(GenerateLeadingWhitespace(indentOffset, out wsLength));
						wr.Write(newLineText);
						offset = wsLength + newLineText.Length;
					}
				}
				comment = remainingText;
			}
		}

		/// <summary>
		///   Converts a template string to an actual string (template items implemented).
		/// </summary>
		/// <param name="template">Template string.</param>
		/// <param name="fileName">Name of the file that will be containing the template text. Can be null.</param>
		/// <param name="description">Description of the file that will be containing the template text. Can be null.</param>
		/// <param name="isCopyRight">True if the line is part of the copyright template (prevents endless cycles).</param>
		/// <returns>String with the template items replaced.</returns>
		/// <exception cref="ArgumentNullException">template is a null reference.</exception>
		public static string ConvertTemplateLineToActual(string template, string fileName, string description, bool? isCopyRight = null, bool? isLicense = null)
		{
			if (template == null)
				throw new ArgumentNullException("template");

			if (template.Length == 0)
				return template;

			if (fileName == null)
				fileName = string.Empty;
			if (description == null)
				description = string.Empty;

			// Generic template items.
			if (template.Contains("<%year%>"))
				template = template.Replace("<%year%>", DateTime.Now.Year.ToString());
			if (template.Contains("<%date%>"))
				template = template.Replace("<%date%>", DateTime.Now.ToShortDateString());
			if (template.Contains("<%time%>"))
				template = template.Replace("<%time%>", DateTime.Now.ToLongTimeString());
			if (template.Contains("<%datetime%>"))
				template = template.Replace("<%datetime%>", DateTime.Now.ToString());

			// Global items.
			if (template.Contains("<%developer%>"))
				template = template.Replace("<%developer%>", DefaultValues.Developer);
			if (template.Contains("<%company%>"))
				template = template.Replace("<%company%>", DefaultValues.CompanyName);
			if (template.Contains("<%appversion%>"))
				template = template.Replace("<%appversion%>", DefaultValues.ApplicationVersion);
			if (template.Contains("<%appname%>"))
				template = template.Replace("<%appname%>", DefaultValues.ApplicationName);
			if (template.Contains("<%libraryversion%>"))
				template = template.Replace("<%libraryversion%>", DefaultValues.LibraryVersion);
			if (template.Contains("<%libraryname%>"))
				template = template.Replace("<%libraryname%>", DefaultValues.LibraryName);
			if (template.Contains("<%copyright%>"))
			{
				if (isCopyRight.HasValue && isCopyRight.Value)
					template = template.Replace("<%copyright%>", string.Empty); // Avoid an infinite loop.
				else
				{
					string copyRight = ConvertTemplateLineToActual(DefaultValues.CopyrightTemplate, fileName, description, true, isLicense);
					template = template.Replace("<%copyright%>", copyRight);
				}
			}
			if (template.Contains("<%license%>"))
			{
				if (isLicense.HasValue && isLicense.Value)
					template = template.Replace("<%license%>", string.Empty); // Avoid an infinite loop.
				else
				{
					StringBuilder sb = new StringBuilder();
					foreach (string line in DefaultValues.LicenseTemplate)
						sb.AppendLine(ConvertTemplateLineToActual(line, fileName, description, isCopyRight, true));
					template = template.Replace("<%license%>", sb.ToString());
				}
			}


			// File Specific items.
			if (template.Contains("<%filename%>"))
				template = template.Replace("<%filename%>", fileName);
			if (template.Contains("<%description%>"))
				template = template.Replace("<%description%>", description);

			return template;
		}

		/// <summary>
		///   Generates a flower box text, based on the given number of indentations.
		/// </summary>
		/// <param name="indentOffset">Number of indentations before the flower box text begins.</param>
		/// <returns>String containing the entire text (whitespace and flower box).</returns>
		private static string GenerateFullFlowerLine(int indentOffset)
		{
			int size;
			string whitespace = GenerateLeadingWhitespace(indentOffset, out size);
			string line = GenerateRemainingFlowerLine(size);
			return string.Format("{0}{1}", whitespace, line);
		}

		/// <summary>
		///   Generates the leading whitespace based on the specified number of indents.
		/// </summary>
		/// <param name="indents">Number of indentations to include in the whitespace. These typically represent the number of tabs.</param>
		/// <returns>String representing the whitespace.</returns>
		/// <remarks>If using spaces instead of tabs, the <see cref="DefaultValues.TabSize"/> value represents the number of spaces per indentation.</remarks>
		public static string GenerateLeadingWhitespace(int indents)
		{
			int num;
			return GenerateLeadingWhitespace(indents, out num);
		}

		public static string GenerateSpaces(int number)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < number; i++)
				sb.Append(" ");
			return sb.ToString();
		}

		/// <summary>
		///   Generates the leading whitespace based on the specified number of indents.
		/// </summary>
		/// <param name="indents">Number of indentations to include in the whitespace. These typically represent the number of tabs.</param>
		/// <param name="numCharacters">Number of characters the whitespace contains.</param>
		/// <returns>String representing the whitespace.</returns>
		/// <remarks>If using spaces instead of tabs, the <see cref="DefaultValues.TabSize"/> value represents the number of spaces per indentation.</remarks>
		public static string GenerateLeadingWhitespace(int indents, out int numCharacters)
		{
			// Generate the leading whitespace for each text.
			StringBuilder sb = new StringBuilder();
			numCharacters = 0;
			for (int i = 0; i < indents; i++)
			{
				numCharacters += DefaultValues.TabSize;
				if (DefaultValues.UseTabs)
				{
					sb.Append("	");
				}
				else
				{
					for (int j = 0; j < DefaultValues.TabSize; j++)
						sb.Append(" ");
				}
			}
			return sb.ToString();
		}

		/// <summary>
		///   Generates the a flower text to consume the rest of the text.
		/// </summary>
		/// <param name="numCharactersConsumed">Number of characters already in the text.</param>
		/// <returns>String containing the flower text.</returns>
		private static string GenerateRemainingFlowerLine(int numCharactersConsumed)
		{
			int numFlowers = DefaultValues.NumCharactersPerLine - 2 - numCharactersConsumed;
			if (numFlowers <= 0)
				return string.Empty;
			StringBuilder sb = new StringBuilder();
			sb.Append("--");
			for (int i = 0; i < numFlowers; i++)
				sb.Append(DefaultValues.FlowerBoxCharacter.Value);
			return sb.ToString();
		}

		/// <summary>
		///   Gets information about places to split in the line.
		/// </summary>
		/// <param name="line">Line to obtain information about.</param>
		/// <param name="stringLookup">Lookup table containing the start location and length of strings in the line.</param>
		/// <param name="semiColons">Indexes of semicolon signs in the line (not including string sections).</param>
		/// <param name="comma">Indexes of commas in the line (not including string sections).</param>
		private static void GetLineSplitInfo(string line, out Dictionary<int, int> stringLookup, out List<int> semiColons, out List<int> comma)
		{
			// Find the locations in the line where strings are, commas, and equal signs. These are the areas we will split on.
			stringLookup = new Dictionary<int, int>();
			semiColons = new List<int>();
			comma = new List<int>();
			bool inString = false;
			int start = 0;
			for (int i = 0; i < line.Length; i++)
			{
				if (line[i] == '"')
				{
					if (inString)
					{
						// If inString is true then must not be in position 0 so we don't need to worry about i-1.
						// Only assume end of string if the '\' character does not preceed it.
						if (line[i - 1] != '\\')
						{
							stringLookup.Add(start, i - start);
							inString = false;
						}
					}
					else
					{
						start = i;
						inString = true;
					}
				}
				else if (line[i] == ';')
				{
					if (!inString)
						semiColons.Add(i);
				}
				else if (line[i] == ',')
				{
					if (!inString)
						comma.Add(i);
				}
			}
		}

		/// <summary>
		///   Determines whether the <paramref name="index"/> is located in the string based on the provided <paramref name="stringLookup"/>.
		/// </summary>
		/// <param name="stringLookup">Lookup table of the start and lengths of the string sections in the line.</param>
		/// <param name="index">Index into the line to check to see if it is in a string.</param>
		/// <returns>True if <paramref name="index"/> is in a string section, false otherwise.</returns>
		private static bool IsInString(Dictionary<int, int> stringLookup, int index)
		{
			foreach (int start in stringLookup.Keys)
			{
				if (start > index)
					return false; // Can't find one that works.
				if (start + stringLookup[start] >= index)
					return true;
			}
			return false;
		}

		/// <summary>
		///   Splits the line, if possible, at valid points.
		/// </summary>
		/// <param name="line">Line to be split.</param>
		/// <param name="numIndents">>Number of indentations to include before the text.</param>
		/// <returns>Array of new lines provided by the split.</returns>
		private static string[] SplitLine(string line, int numIndents)
		{
			// Check for tabs or spaces at the beginning of the line.
			int numCharacters = 0;
			int additionalWhiteSpaceSize = 0;
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < line.Length; i++)
			{
				if (line[i] == ' ')
				{
					numCharacters++;
					additionalWhiteSpaceSize++;
					sb.Append(' ');
				}
				else if (line[i] == '\t')
				{
					numCharacters++;
					additionalWhiteSpaceSize += DefaultValues.TabSize;
					sb.Append('\t');
				}
				else
				{
					break;
				}
			}
			string additionalWhiteSpace = sb.ToString();
			line = line.Substring(numCharacters);

			// Find the locations in the line where strings are, commas, and equal signs. These are the areas we will split on.
			Dictionary<int, int> stringLookup;
			List<int> equalSign;
			List<int> comma;
			GetLineSplitInfo(line, out stringLookup, out equalSign, out comma);

			List<string> newLines = new List<string>();
			bool firstSplit = true;
			int whiteSpaceSize = numIndents * DefaultValues.TabSize;
			string ws = GenerateLeadingWhitespace(numIndents);
			while (whiteSpaceSize + additionalWhiteSpaceSize + line.Length > DefaultValues.NumCharactersPerLine)
			{
				// Find a good split location.
				int splitLocation = 0;
				bool found = false;
				bool splitInString = false;
				for (int i = DefaultValues.NumCharactersPerLine - whiteSpaceSize - additionalWhiteSpaceSize - 1; i > 0; i--)
				{
					// Check for equals.
					if (equalSign.Contains(i) || comma.Contains(i))
					{
						splitLocation = i;
						found = true;
						break;
					}
					if (IsInString(stringLookup, i) && line[i] == ' ')
					{
						splitLocation = i;
						found = true;
						splitInString = true;
						break;
					}
				}

				if (!found)
					break; // Couldn't find a break point.

				if (splitInString)
				{
					newLines.Add(string.Format("{0}{1}{2}\"", ws, additionalWhiteSpace, line.Substring(0, splitLocation)));
					line = string.Format("& \"{0}", line.Substring(splitLocation));
					GetLineSplitInfo(line, out stringLookup, out equalSign, out comma);
				}
				else
				{
					newLines.Add(string.Format("{0}{1}{2}", ws, additionalWhiteSpace, line.Substring(0, splitLocation + 1)));
					if (line[splitLocation + 1] == ' ')
						line = line.Substring(splitLocation + 2); // Skip the space if found.
					else
						line = line.Substring(splitLocation + 1);
					GetLineSplitInfo(line, out stringLookup, out equalSign, out comma);
				}

				if (firstSplit)
				{
					additionalWhiteSpaceSize += DefaultValues.TabSize;
					additionalWhiteSpace = string.Format("{0}{1}", additionalWhiteSpace, GenerateLeadingWhitespace(1));
					firstSplit = false;
				}
			}

			// Finally add the last line.
			newLines.Add(string.Format("{0}{1}{2}", ws, additionalWhiteSpace, line));

			return newLines.ToArray();
		}

		/// <summary>
		///   Splits the comment text on a space between words, or splits a word and hyphens it.
		/// </summary>
		/// <param name="text">Text to evaluate for splitting.</param>
		/// <param name="lineOffset">Offset in the line where the text will start.</param>
		/// <param name="lineText">Text to be written to the line. Can be empty if there is no room to write any text.</param>
		/// <param name="remainingText">Text remaining that could not fit on the line. Can be null if no text is remaining.</param>
		private static void SplitOnEndOfLine(string text, int lineOffset, out string lineText, out string remainingText)
		{
			// remove any leading or trailing whitespace.
			text = text.Trim();

			int remainingSpace = DefaultValues.NumCharactersPerLine - lineOffset - 1;
			if (text.Length <= remainingSpace)
			{
				// The text will fit in the space.
				lineText = text;
				remainingText = null;
				return;
			}

			if (remainingSpace < 1)
			{
				// Not enough space to add any text.
				lineText = string.Empty;
				remainingText = text;
				return;
			}

			if (remainingSpace == 1)
			{
				// Might not be enough space if we need a hyphen.
				if (text[1] == ' ')
				{
					// Add single character to text.
					lineText = text[0].ToString();
					remainingText = text.Substring(2);
				}
				else
				{
					// Not enough space to add one character and hyphen.
					lineText = string.Empty;
					remainingText = text;
				}
			}

			int splitIndex = text.LastIndexOf(' ', remainingSpace - 1, remainingSpace);
			if (splitIndex == -1)
			{
				// Break the file on the end of the text and place a hyphen.
				lineText = string.Format("{0}-", text.Substring(0, remainingSpace - 1)); // Should fill remaining space.
				remainingText = text.Substring(remainingSpace - 1);
			}
			else
			{
				lineText = text.Substring(0, splitIndex);
				remainingText = text.Substring(splitIndex + 1);
			}
			return;
		}

		/// <summary>
		///   Writes text to the <see cref="StreamWriter"/> object. Does not add any whitespace.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <param name="line">Text to be written.</param>
		/// <returns>Number of characters that were written.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> or <paramref name="text"/> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static int Write(StreamWriter wr, string line)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (line == null)
				throw new ArgumentNullException("text");
			wr.Write(line);
			return line.Length;
		}

		/// <summary>
		///   Writes text to the <see cref="StreamWriter"/> object (including whitespace).
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <param name="text">Text to be written.</param>
		/// <param name="indentOffset">Number of indentations to include before the text.</param>
		/// <returns>Number of characters that were written.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> or <paramref name="text"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="text"/> is an empty sting.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static int Write(StreamWriter wr, string text, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (text == null)
				throw new ArgumentNullException("text");
			if (text.Length == 0)
				throw new ArgumentException("text is an empty string");

			int numWhiteSpace;
			string whiteSpace = GenerateLeadingWhitespace(indentOffset, out numWhiteSpace);

			if (numWhiteSpace + text.Length > DefaultValues.NumCharactersPerLine)
			{
				// Line is too long so split it into multiple lines if possible.
				string[] splits = SplitLine(text, indentOffset);
				int size = 0;
				foreach (string split in splits)
				{
					wr.WriteLine(split);
					size += split.Length;
				}
				return size;
			}
			wr.Write(string.Format("{0}{1}", whiteSpace, text));
			return numWhiteSpace + text.Length;
		}

		/// <summary>
		///   Generates a file header for a VHDL file.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the file header to.</param>
		/// <param name="fileName">Name of thee VHDL file to generate the header for.</param>
		/// <param name="description">Description of the file. Can be null.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> or <paramref name="fileName"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="fileName"/> is an empty string.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static void WriteFileHeader(StreamWriter wr, string fileName, string description = null)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (fileName.Length == 0)
				throw new ArgumentException("fileName is an empty string");

			// Write the file information section.
			if (DefaultValues.FileInfoTemplate != null && DefaultValues.FileInfoTemplate.Length > 0)
			{
				foreach (string templateLine in DefaultValues.FileInfoTemplate)
					WriteLine(wr, ConvertTemplateLineToActual(templateLine, fileName, description), 0);
			}

			// Write the copyright statement line.
			if (DefaultValues.CopyrightTemplate != null && DefaultValues.CopyrightTemplate.Length > 0)
			{
				WriteLine(wr, string.Format("-- {0}", ConvertTemplateLineToActual(DefaultValues.CopyrightTemplate, fileName, description, true)), 0);
			}

			// Write the license section.
			if (DefaultValues.LicenseTemplate != null && DefaultValues.LicenseTemplate.Length > 0)
			{
				foreach (string line in DefaultValues.LicenseTemplate)
					WriteLine(wr, string.Format("-- {0}", ConvertTemplateLineToActual(line, fileName, description)), 0);
			}
		}

		/// <summary>
		///   Writes a flower box text with the specified indentation number to the <see cref="StreamWriter"/>.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <param name="indentOffset">Number of indentations to add to the text.</param>
		/// <returns>True if a line was written, false otherwise. A line is not written if the <see cref="DefaultValues.FlowerBoxCharacter"/> is null.</returns>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static bool WriteFlowerLine(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			// Don't do anything if the flower box character is not defined.
			if (DefaultValues.FlowerBoxCharacter == null)
				return false;

			wr.WriteLine(GenerateFullFlowerLine(indentOffset));
			return true;
		}

		/// <summary>
		///   Creates a generalized documentation section.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the documentation to.</param>
		/// <param name="documentation">Lookup table containing the section names (keys) and array of values (text in that section).</param>
		/// <param name="indentOffset">Number of indentations, before the code/comments start.</param>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static void WriteGeneralDocumentationElements(StreamWriter wr, Dictionary<string, string[]> documentation, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (documentation == null)
				throw new ArgumentNullException("documentation");

			if (documentation.Count == 0)
				return;

			int maxLength = 0;
			foreach(string key in documentation.Keys)
			{
				if (key.Length > maxLength)
					maxLength = key.Length;
			}

			int wsLength;
			string ws = GenerateLeadingWhitespace(indentOffset, out wsLength);
			string newLine = string.Format("-- {0}  ", GenerateSpaces(maxLength));

			foreach (string key in documentation.Keys)
			{
				string additionalKeySpace = GenerateSpaces(maxLength - key.Length);
				if (documentation[key].Length == 1)
				{
					if (wsLength + newLine.Length + documentation[key][0].Length > DefaultValues.NumCharactersPerLine)
					{
						wr.Write(string.Format("{0}-- {1}: {2}", ws, key, additionalKeySpace));
						AddCommentToEndOfLine(wr, documentation[key][0], newLine.Length, indentOffset, newLine);
					}
					else
					{
						// Place all on the same line as the key.
						wr.WriteLine(string.Format("{0}-- {1}: {2}{3}", ws, key, additionalKeySpace, documentation[key][0]));
					}
				}
				else
				{
					// Create starting line.
					wr.WriteLine(string.Format("{0}-- {1}:", ws, key, additionalKeySpace));
					for (int i = 0; i < documentation[key].Length; i++)
					{
						if (wsLength + newLine.Length + documentation[key][i].Length > DefaultValues.NumCharactersPerLine)
						{
							wr.Write(newLine);
							AddCommentToEndOfLine(wr, documentation[key][i], newLine.Length, indentOffset, newLine);
						}
						else
						{
							// Place on the same line.
							wr.WriteLine(string.Format("{0}{1}", newLine, documentation[key][i]));
						}
					}
				}
			}
		}

		/// <summary>
		///   Writes the region start line.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <param name="nameOfRegion">Name of the region.</param>
		/// <param name="indentOffset">Number of indentations to include before the text.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> or <paramref name="nameOfRegion"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="nameOfRegion"/> is an empty string.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static void WriteRegionStart(StreamWriter wr, string nameOfRegion, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (nameOfRegion == null)
				throw new ArgumentNullException("nameOfRegion");
			if (nameOfRegion.Length == 0)
				throw new ArgumentException("nameOfRegion is an empty string");

			int wsLength;
			string ws = GenerateLeadingWhitespace(indentOffset, out wsLength);
			int middleIndex = ((DefaultValues.NumCharactersPerLine - wsLength) / 2) - 1;
			int startIndex = middleIndex - ((nameOfRegion.Length + 2) / 2) + wsLength;

			StringBuilder sb = new StringBuilder();
			sb.Append("--"); // Start of line.

			int index = wsLength + 2;
			for (int i = index; i < startIndex; i++)
			{
				sb.Append(DefaultValues.FlowerBoxCharacter);
				index++;
			}

			// Add the word with spaces on each side.
			sb.AppendFormat(" {0} ", nameOfRegion);
			index += nameOfRegion.Length + 2;

			for (int i = index; i < DefaultValues.NumCharactersPerLine; i++)
				sb.Append(DefaultValues.FlowerBoxCharacter);

			WriteLine(wr, sb.ToString(), indentOffset);
		}

		/// <summary>
		///   Writes a blank text (without whitespace) to the <see cref="StreamWriter"/> object.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static void WriteLine(StreamWriter wr)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			wr.WriteLine();
		}

		/// <summary>
		///   Writes the text to the <see cref="StreamWriter"/> object while adding indentation whitespace.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <param name="line">Line to be written.</param>
		/// <param name="indentOffset">Number of indentations to include before the text.</param>
		/// <returns>Number of characters written on the text.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> or <paramref name="text"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="text"/> is an empty sting.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static int WriteLine(StreamWriter wr, string line, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (line == null)
				throw new ArgumentNullException("text");
			line = line.TrimEnd();
			if (line.Length == 0)
			{
				WriteLine(wr);
				return 0;
			}

			int numWhiteSpace;
			string whiteSpace = GenerateLeadingWhitespace(indentOffset, out numWhiteSpace);

			if (numWhiteSpace + line.Length > DefaultValues.NumCharactersPerLine)
			{
				// Line is too long so split it into multiple lines if possible.
				string[] splits = SplitLine(line, indentOffset);
				int size = 0;
				foreach (string split in splits)
				{
					wr.WriteLine(split);
					size += split.Length;
				}
				return size;
			}
			wr.WriteLine(string.Format("{0}{1}", whiteSpace, line));
			return numWhiteSpace + line.Length;
		}

		#endregion Methods
	}
}