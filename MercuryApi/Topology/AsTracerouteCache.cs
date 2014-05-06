/* 
 * Copyright (C) 2014 Alex Bikfalvi
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
using System.Collections.Generic;
using System.Net;

namespace Mercury.Topology
{
	/// <summary>
	/// A class representing the cache for an AS traceroute.
	/// </summary>
	public sealed class ASTracerouteCache
	{
		/// <summary>
		/// A structure representing the information for an IP address.
		/// </summary>
		public struct IPAddressInformation
		{
			#region Public fields

			/// <summary>
			/// The IP address.
			/// </summary>
			public IPAddress Address;
			/// <summary>
			/// The number of the corresponding autonomous system.
			/// </summary>
			public int ASNumber;

			#endregion
		}

		private readonly ASTracerouteSettings settings;

		private readonly Dictionary<IPAddress, IPAddressInformation> cache = new Dictionary<IPAddress, IPAddressInformation>();

		/// <summary>
		/// Creates a new AS traceroute cache.
		/// </summary>
		/// <param name="settings">The settings.</param>
		public ASTracerouteCache(ASTracerouteSettings settings)
		{
			this.settings = settings;
		}

		#region Public methods

		/// <summary>
		/// Adds to the cache the information for the specified list of IP addresses.
		/// </summary>
		/// <param name="addresses">The list of IP addresses.</param>
		public void Add(IEnumerable<IPAddress> addresses)
		{

		}

		/// <summary>
		/// Gets from the cache the information for the specified IP address.
		/// </summary>
		/// <param name="address">The IP address.</param>
		/// <returns>The address information.</returns>
		//public IPAddressInformation Get(IPAddress address)
		//{
		//	IPAddressInformation info;

		//	// Try and get the value from the cache.
		//	if (this.cache.TryGetValue(address, out info))
		//	{
		//		return info;
		//	}
		//	else
		//	{

		//	}
		//}

		#endregion

		#region Private methods

		/// <summary>
		/// Updates the cache information for the specified IP address.
		/// </summary>
		/// <param name="address">The IP address.</param>
		/// <returns>The IP address information.</returns>
		//private IPAddressInformation Update(IPAddress address)
		//{
		//	return null;
		//}

		#endregion
	}
}
