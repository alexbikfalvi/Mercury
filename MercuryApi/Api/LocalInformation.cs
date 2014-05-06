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
using System.Net;
using Newtonsoft.Json.Linq;

namespace MercuryApi.Api
{
	/// <summary>
	/// A class representing the local information.
	/// </summary>
	public class LocalInformation
	{
		/// <summary>
		/// Private constructor.
		/// </summary>
		private LocalInformation()
		{
		}

		#region Public properties

		/// <summary>
		/// Gets the IP address.
		/// </summary>
		public IPAddress Address { get; private set; }
		/// <summary>
		/// Gets the AS number.
		/// </summary>
		public uint ASNumber { get; private set; }
		/// <summary>
		/// Gets the AS name.
		/// </summary>
		public string ASName { get; private set; }
		/// <summary>
		/// Gets the timestamp.
		/// </summary>
		public DateTime Timestamp { get; private set; }

		#endregion

		#region Internal methods

		/// <summary>
		/// Parses a local information object from the specified JSON object.
		/// </summary>
		/// <param name="json">The JSON object.</param>
		/// <returns>The local information object.</returns>
		internal static LocalInformation Parse(JObject json)
		{
			// Create the object.
			LocalInformation info = new LocalInformation();

			// Set the properties.
			//info.Address = IPAddress.Parse(json["ip"].);

			// Return the object.
			return info;
		}

		#endregion
	}
}
