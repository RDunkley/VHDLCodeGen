//********************************************************************************************************************************
// Filename:    ComponentInfo.cs
// Owner:       Richard Dunkley
// Description: Defines the various architectural types.
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
	///   Enumerates the various architecture types for a module.
	/// </summary>
	public enum ArchitecturalType
	{
		/// <summary>
		///   Represents an behavioral architecture.
		/// </summary>
		Behavioral,

		/// <summary>
		///   Represents an Register Transfer Level (RTL) architecture.
		/// </summary>
		RTL,
	}
}
