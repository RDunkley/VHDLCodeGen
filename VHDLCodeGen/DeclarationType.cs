//********************************************************************************************************************************
// Filename:    DeclarationType.cs
// Owner:       Richard Dunkley
// Description: Enumerates the various types that a declaration can represent.
//********************************************************************************************************************************
// Copyright © Richard Dunkley 2016
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0  Unless required by applicable
// law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
// limitations under the License.
//********************************************************************************************************************************
namespace VHDLCodeGen
{
	/// <summary>
	///   Defines the type of declarations made in the architectural portion of an entity.
	/// </summary>
	public enum DeclarationType
	{
		#region Names

		/// <summary>
		///   Represents the VHDL constant declaration.
		/// </summary>
		Constant,

		/// <summary>
		///   Represents the VHDL sub-type declaration.
		/// </summary>
		SubType,

		/// <summary>
		///   Represents the VHDL type declaration.
		/// </summary>
		Type,

		#endregion Names
	}
}
