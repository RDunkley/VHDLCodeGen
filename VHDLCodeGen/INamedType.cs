//********************************************************************************************************************************
// Filename:    INamedType.cs
// Owner:       Richard Dunkley
// Description: Contains the interface for named types.
//********************************************************************************************************************************
// Copyright © Richard Dunkley 2017
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
	///   Defines the interface for Named Types.
	/// </summary>
	public interface INamedType
	{
		#region Properties

		/// <summary>
		///   Name of the type.
		/// </summary>
		string Name { get; }

		#endregion Properties
	}
}
