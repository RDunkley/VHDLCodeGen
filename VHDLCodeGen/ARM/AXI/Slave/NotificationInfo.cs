//********************************************************************************************************************************
// Filename:    NotificationInfo.cs
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
using System.Text;

namespace VHDLCodeGen.ARM.AXI.Slave
{
	/// <summary>
	///   Represents a notification in the addressable register space.
	/// </summary>
	/// <remarks>
	///   A notification is a single cycle clock pulse that occurs when a register or multiple registers are written
	///   or read from.
	/// </remarks>
	public class NotificationInfo : PartialRegisterInfo
	{
		#region Properties

		/// <summary>
		///   Gets or sets the signal name of the internal notification signal when a read is performed.
		/// </summary>
		public string ReadSignalName { get; set; }

		/// <summary>
		///   Gets or sets the signal name of the internal notification signal when a write is performed.
		/// </summary>
		public string WriteSignalName { get; set; }

		#endregion

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="NotificationInfo"/> object.
		/// </summary>
		/// <param name="designator">Designator of the notification. Must be unique in the system.</param>
		/// <param name="offset">
		///   Offset in the register block that the notification will be asserted from (notification area). Must fall on a
		///   register boundary.
		/// </param>
		/// <param name="length">Length of the notification area in bytes. Must be a multiple of the register width.</param>
		/// <param name="access">
		///   Accessibility of the notification area in the register space. A separate notification is created for reads and writes.
		/// </param>
		/// <param name="name">Human readable name of the block.</param>
		/// <exception cref="ArgumentException">
		///   <paramref name="access"/> is unrecognized, or the length is less than 4.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="designator"/> is a null reference.
		/// </exception>
		public NotificationInfo(string designator, ulong offset, Access access, int fullRegisterWidth,
			int start, int end, string name = null)
			: base("notification", designator, offset, access, start, end, fullRegisterWidth, name)
		{
			ReadSignalName = $"{designator}_rd_ntfy";
			WriteSignalName = $"{designator}_wr_ntfy";
		}

		#endregion
	}
}
