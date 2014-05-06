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

namespace Mercury.Topology
{
	/// <summary>
	/// A class for autonomous system traceroute settings.
	/// </summary>
	public sealed class ASTracerouteSettings
	{
		/// <summary>
		/// Creates a new AS traceroute settings instance.
		/// </summary>
		public ASTracerouteSettings()
		{
			this.IpToAsnUrl = "http://mercury.upf.edu/mercury/api/services/getIp2AsnMappingsByIpsPOST";
			this.IpToAsnMaximum = 1000;
		}

		#region Public properties

		/// <summary>
		/// Gets or sets the service URL for translating IP address to AS number.
		/// </summary>
		public string IpToAsnUrl { get; set; }
		/// <summary>
		/// Gets or sets the maximum number of addresses that can be translated at a time.
		/// </summary>
		public int IpToAsnMaximum { get; set; }

		#endregion
	}
}
