//********************************************************************************************************************************
// Filename:    SimplifiedPortInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components of a VHDL port.
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
using System.IO;

namespace VHDLCodeGen
{
	/// <summary>
	///   Represents a simplified VHDL port.
	/// </summary>
	public class SimplifiedPortInfo
	{
		#region Properties

		/// <summary>
		///   Default value of the port. Can be null or empty.
		/// </summary>
		public string DefaultValue { get; set; }

		/// <summary>
		///   Direction of the port.
		/// </summary>
		public PortDirection Direction { get; protected set; }

		/// <summary>
		///   Name of the port.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		///   Type of the port.
		/// </summary>
		public string Type { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="SimplifiedPortInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the port.</param>
		/// <param name="direction"><see cref="PortDirection"/> describing the direction of the port.</param>
		/// <param name="type">Type of the port.</param>
		/// <param name="defaultValue">Default value of the port. Can be null or empty.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/>, or <paramref name="type"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/>, or <paramref name="type"/> is an empty string.</exception>
		public SimplifiedPortInfo(string name, PortDirection direction, string type, string defaultValue = null)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string");
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.Length == 0)
				throw new ArgumentException("type is an empty string");


			Name = name;
			Direction = direction;
			DefaultValue = defaultValue;
			Type = type;
		}

		/// <summary>
		///   Gets the declaration of the port.
		/// </summary>
		/// <returns>Declaration of the port.</returns>
		public string GetDeclaration()
		{
			string defaultValueString = string.Empty;
			if (!string.IsNullOrWhiteSpace(DefaultValue))
				defaultValueString = string.Format(" := {0}", DefaultValue);

			return string.Format("{0} : {1} {2}{3}", Name, Enum.GetName(typeof(PortDirection), Direction).ToLower(), Type, defaultValueString);
		}

		/// <summary>
		///   Writes the port declaration to a stream.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the declaration to.</param>
		/// <param name="ports">Array of <see cref="SimplifiedPortInfo"/> objects to write in the declaration.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/>, or <paramref name="ports"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="ports"/> is an empty array.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static void WritePortDeclaration(StreamWriter wr, SimplifiedPortInfo[] ports, int indentOffset)
		{
			if (ports == null)
				throw new ArgumentNullException("generics");
			if (ports.Length == 0)
				throw new ArgumentException("generics is an empty array");

			DocumentationHelper.WriteLine(wr, "port (", indentOffset);
			string ending = ";";
			for (int i = 0; i < ports.Length; i++)
			{
				if (i == ports.Length - 1)
					ending = string.Empty;
				DocumentationHelper.WriteLine(wr, string.Format("{0}{1}", ports[i].GetDeclaration(), ending), indentOffset + 1);
			}
			DocumentationHelper.WriteLine(wr, ");", indentOffset);
		}

		#endregion Methods
	}
}
