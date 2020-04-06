//********************************************************************************************************************************
// Filename:    InterfaceType.cs
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

namespace VHDLCodeGen.ARM.AXI
{
	/// <summary>
	/// Enumerates the various types of AXI bus interfaces.
	/// </summary>
	public enum InterfaceType
	{
		/// <summary>
		///  The interface is a master.
		/// </summary>
		/// <remarks>This interface will direct the read and write transactions on the bus.</remarks>
		Master,

		/// <summary>
		///   The interface is a slave.
		/// </summary>
		/// <remarks>This interface will receive the read and write transactions on the bus directed towards it.</remarks>
		Slave,

		/// <summary>
		///   The interface is a monitor.
		/// </summary>
		/// <remarks>This interface will monitor a AXI bus without performing any transaction upon it.</remarks>
		Monitor,
	}
}
