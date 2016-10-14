//********************************************************************************************************************************
// Filename:    ProcedureParameterType.cs
// Owner:       Richard Dunkley
// Description: Contains an enumeration of the various types a procedure parameter can be.
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
	///   Defines the types that a procedure's parameter may be.
	/// </summary>
	public enum ProcedureParameterType
	{
		#region Names

		/// <summary>
		///   Parameter is a constant. Default type for <see cref="PortDirection.In"/>.
		/// </summary>
		Constant,

		/// <summary>
		///   Parameter is a signal.
		/// </summary>
		Signal,

		/// <summary>
		///   Parameter is a variable. Default type for <see cref="PortDirection.Out"/> and <see cref="PortDirection.InOut"/>.
		/// </summary>
		Variable,

		#endregion Names
	}
}
