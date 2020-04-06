//********************************************************************************************************************************
// Filename:    SignalInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components for a VHDL signal.
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
	///   Contains information about a signal.
	/// </summary>
	public class SignalInfo : BaseTypeInfo
	{
		#region Properties

		/// <summary>
		///   Default value of the signal. Can be null or empty.
		/// </summary>
		public string DefaultValue { get; set; }

		/// <summary>
		///   Type of the signal (Ex: <i>unsigned(7 downto 0)</i>, <i>integer</i>, <i>integer range 0 to 1</i> etc.).
		/// </summary>
		public string Type { get; protected set; }

		/// <summary>
		///   When true the signal represents the actual signal and a pre-clocked signal (Ex: sig, next_sig).
		/// </summary>
		public bool AddPreClockSignal { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="SignalInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the signal.</param>
		/// <param name="type">Type of the signal.</param>
		/// <param name="summary">Summary description of the signal.</param>
		/// <param name="defaultValue">Default value of the signal. Can be null or empty.</param>
		/// <param name="addPreClockSignal">True if the signal represents the actual signal and a pre-clocked signal (Ex: sig, next_sig).</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="type"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/>, <paramref name="type"/>, or <paramref name="summary"/> is an empty string.</exception>
		public SignalInfo(string name, string type, string summary, string defaultValue = null, string remarks = null, bool addPreClockSignal = false)
			: base(name, summary, remarks)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.Length == 0)
				throw new ArgumentException("type is an empty string");

			Type = type;
			DefaultValue = defaultValue;
			AddPreClockSignal = addPreClockSignal;
		}

		/// <summary>
		///   Writes the signal to a stream.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the signal to.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public override void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			if (indentOffset < 0)
				indentOffset = 0;

			string defaultValueString = string.Empty;
			if (!string.IsNullOrWhiteSpace(DefaultValue))
				defaultValueString = string.Format(" := {0}", DefaultValue);

			string nextValueString = string.Empty;
			if (AddPreClockSignal)
				nextValueString = string.Format(", next_{0}", Name);

			// Write the header.
			WriteBasicHeader(wr, indentOffset);
			DocumentationHelper.WriteLine(wr, string.Format("signal {0}{1} : {2}{3};", Name, nextValueString, Type, defaultValueString), indentOffset);
		}

		#endregion Methods
	}
}
