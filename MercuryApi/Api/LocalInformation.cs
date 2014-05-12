/* 
 * Copyright (C) 2014 Alex Bikfalvi, Manuel Palacin
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or (at
 * your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;

namespace Mercury.Api
{
	/// <summary>
	/// A class representing the local information.
	/// </summary>
	[DataContract]
	public class LocalInformation
	{
		[DataMember]
		internal string ipAddress;
		[DataMember]
		internal string asNumber;
		[DataMember]
		internal string asName;
		[DataMember]
		internal string timestamp;

		#region Public properties

		/// <summary>
		/// Gets the IP address.
		/// </summary>
		public IPAddress Address { get { return IPAddress.Parse(this.ipAddress); } }
		/// <summary>
		/// Gets the AS number.
		/// </summary>
		public uint ASNumber { get { return uint.Parse(this.asNumber); } }
		/// <summary>
		/// Gets the AS name.
		/// </summary>
		public string ASName { get { return this.asName; } }
		/// <summary>
		/// Gets the timestamp.
		/// </summary>
		public DateTime Timestamp { get { return DateTime.Parse(this.timestamp); } }

		#endregion
	}
}
