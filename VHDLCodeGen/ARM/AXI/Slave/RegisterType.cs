//********************************************************************************************************************************
// Filename:    RegisterType.cs
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
namespace VHDLCodeGen.ARM.AXI.Slave
{
	/// <summary>
	///   Enumerate the various types of registers that can be added to the AXI register space.
	/// </summary>
	public enum RegisterType
	{
		/// <summary>
		///   Internal register. This represents a standard logic vector register component that will be read and/or written to from the
		///   AXI memory interface. If the register is read-only then it will reference the register name, but will not create it. If the
		///   register is writeable then an internal register will be created that can be read and written to.
		/// </summary>
		StandardLogic = 0,

		/// <summary>
		///   Internal unsigned register. This represents an unsigned register component that will be read and/or written to from the AXI
		///   memory interface, but is unsigned instead of a standard logic vector. If the register is read-only then it will reference
		///   the register name, but will not create it. If the register is writeable then an internal register will be created that can
		///   be read and written to.
		/// </summary>
		Unsigned = 1,
	}
}
