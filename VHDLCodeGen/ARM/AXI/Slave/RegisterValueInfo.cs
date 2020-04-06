//********************************************************************************************************************************
// Filename:    RegisterValueInfo.cs
// Owner:       Richard Dunkley
//********************************************************************************************************************************
// Copyright © Richard Dunkley 2020
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0  Unless required by applicable
// law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
// limitations under the License.
//********************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VHDLCodeGen.ARM.AXI.Slave
{
	/// <summary>
	///   Represents a register value in the addressable register space.
	/// </summary>
	/// <remarks>
	///   The register value can represent an entire register or a smaller portion of a single register. Size cannot exceed the single register width.
	/// </remarks>
	public class RegisterValueInfo : PartialRegisterInfo
	{
		#region Properties

		/// <summary>
		///   Gets or sets the default value of the register.
		/// </summary>
		/// <remarks>A default value is provided, but can be changed as required. Only applicable on certain register types.</remarks>
		public string DefaultValue { get; set; }

		/// <summary>
		///   Gets or sets the name of the internal signal for the register.
		/// </summary>
		/// <remarks>A default name is provided, but can be changed as required. Only applicable on certain register types.</remarks>
		public string SignalName { get; set; }

		/// <summary>
		///   Gets or sets the type of the internal signal for the register.
		/// </summary>
		/// <remarks>A default type is provided, but can be changed as required. Only applicable on certain register types.</remarks>
		public string SignalType { get; set; }

		/// <summary>
		///   Gets the register type associated with the register value.
		/// </summary>
		public RegisterType Type { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="RegisterValueInfo"/> object.
		/// </summary>
		/// <param name="designator">Unique designator of the register in the register space.</param>
		/// <param name="type"><see cref="RegisterType"/> of the register.</param>
		/// <param name="offset">Offset of the register in the register space. Must fall on a register boundary.</param>
		/// <param name="accessibility">Accessibility of the register in the register space.</param>
		/// <param name="fullRegisterWidth">Full width of the register (in bits).</param>
		/// <param name="start">Index of the starting bit in the register.</param>
		/// <param name="end">Index of the ending bit in the register.</param>
		/// <param name="name">Human readable name of the register.</param>
		/// <exception cref="ArgumentException">
		///   <paramref name="accessibility"/> is unrecognized.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="designator"/> is a null reference.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///   <paramref name="fullRegisterWidth"/> is not equal to 32 or 64, <paramref name="start"/> or <paramref name="end"/> are less than 0 or
		///   greater than <paramref name="fullRegisterWidth"/> - 1.
		/// </exception>
		public RegisterValueInfo(string designator, RegisterType type, ulong offset, Access accessibility, int fullRegisterWidth, 
			int start, int end, string name = null)
			: base("register value", designator, offset, accessibility, start, end, fullRegisterWidth, name)
		{
			Type = type;
			SignalName = designator;
			switch(type)
			{
				case RegisterType.StandardLogic:
					SignalType = VHDLUtility.GetStdLogicType(StartBit, EndBit, true);
					DefaultValue = VHDLUtility.GetBitString(0, EndBit - StartBit + 1, true);
					break;
				case RegisterType.Unsigned:
					SignalType = VHDLUtility.GetUnsignedType(StartBit, EndBit, true);
					DefaultValue = VHDLUtility.GetBitString(0, EndBit - StartBit + 1, true);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		#endregion
	}
}
