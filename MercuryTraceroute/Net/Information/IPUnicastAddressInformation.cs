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
using System.Net;
using System.Net.NetworkInformation;

namespace Mercury.Net.Information
{
	/// <summary>
	/// A class representing the information for a unicast IP address.
	/// </summary>
	public class IPUnicastAddressInformation : IPAddressInformation
	{
		/// <summary>
		/// Creats a new IP unicast address information instance.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <param name="iface">The interface.</param>
		/// <param name="information">The information.</param>
		public IPUnicastAddressInformation(IPAddress address, NetworkInterface iface, UnicastIPAddressInformation information)
			: base(address, iface)
		{
			this.AddressInformation = information;
		}

		// Public properties.

		/// <summary>
		/// Gets the unicast address information.
		/// </summary>
		public UnicastIPAddressInformation AddressInformation { get; private set; }
	}
}
