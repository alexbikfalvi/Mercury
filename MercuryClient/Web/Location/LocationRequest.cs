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
	/// A class representing a request for the Internet Protocol geo-location information.
	/// </summary>
	public sealed class LocationRequest : AsyncWebRequest
	{
		private static readonly string url = "http://freegeoip.net/xml/{0}";

		/// <summary>
		/// Creates a new location request instance.
		/// </summary>
		public LocationRequest()
		{
		}

		// Public methods.

		/// <summary>
		/// Begins an asynchronous request for the Internet Protocol geo location information.
		/// </summary>
		/// <param name="callback">The callback method.</param>
		/// <param name="userState">The user state.</param>
		/// <returns>The result of the asynchronous operation.</returns>
		public IAsyncResult Begin(AsyncWebRequestCallback callback, object userState)
		{
			return base.Begin(new Uri(string.Format(LocationRequest.url, string.Empty)), callback, userState);
		}

		/// <summary>
		/// Begins an asynchronous request for the Internet Protocol geo location information.
		/// </summary>
		/// <param name="address">The IP address.</param>
		/// <param name="callback">The callback method.</param>
		/// <param name="userState">The user state.</param>
		/// <returns>The result of the asynchronous operation.</returns>
		public IAsyncResult Begin(IPAddress address, AsyncWebRequestCallback callback, object userState)
		{
			return base.Begin(new Uri(string.Format(LocationRequest.url, address.ToString())), callback, userState);
		}

		/// <summary>
		/// Ends an asynchronous request for the Internet Protocol geo location information.
		/// </summary>
		/// <param name="result">The result of the asynchronous operation.</param>
		/// <returns>The location result.</returns>
		public new LocationResult End(IAsyncResult result)
		{
			return base.End<LocationResult>(result, (string data) =>
				{
					return LocationResult.Parse(data);
				});
		}
	}
}
