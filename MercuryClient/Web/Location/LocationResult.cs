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

namespace Mercury.Web.Location
{
	/// <summary>
	/// A class representing the result of a geo-location request.
	/// </summary>
	public sealed class LocationResult
	{
		/// <summary>
		/// Gets the IP address.
		/// </summary>
		public IPAddress Address { get; private set; }
		/// <summary>
		/// Gets the country code.
		/// </summary>
		public string CountryCode { get; private set; }
		/// <summary>
		/// Gets the country name.
		/// </summary>
		public string CountryName { get; private set; }
		public string RegionCode { get; private set; }
		public string RegionName { get; private set; }
		public string City { get; private set; }
		public string ZipCode { get; private set; }
		public double? Latitude { get; private set; }
		public double? Longitude { get; private set; }
		public string MetroCode { get; private set; }
		public string AreaCode { get; private set; }

//		<script/>
//<Ip>193.145.48.8</Ip>
//<CountryCode>ES</CountryCode>
//<CountryName>Spain</CountryName>
//<RegionCode>56</RegionCode>
//<RegionName>Catalonia</RegionName>
//<City>Barcelona</City>
//<ZipCode/>
//<Latitude>41.3984</Latitude>
//<Longitude>2.1741</Longitude>
//<MetroCode/>
//<AreaCode/>
	}
}
