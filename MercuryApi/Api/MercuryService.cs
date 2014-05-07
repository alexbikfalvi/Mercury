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
using System.Runtime.Serialization.Json;

namespace Mercury.Api
{
	/// <summary>
	/// A class representing the Mercury service.
	/// </summary>
	public static class MercuryService
	{
		private static readonly string urlLocalInformation = "http://mercury.upf.edu/mercury/api/services/myInfo";

		#region Public properties

		/// <summary>
		/// Gets the local information from the Mercury service.
		/// </summary>
		/// <returns>The local information.</returns>
		public static LocalInformation GetLocalInformation()
		{
			return MercuryService.Get<LocalInformation>(new Uri(MercuryService.urlLocalInformation));
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Gets from the Mercury service an object of the specified type.
		/// </summary>
		/// <typeparam name="T">The object type.</typeparam>
		/// <param name="uri">The API URI.</param>
		/// <returns>The object deserialized from the JSON response.</returns>
		private static T Get<T>(Uri uri) where T : class
		{
			try
			{
				// Create the request.
				HttpWebRequest request = HttpWebRequest.Create(uri) as HttpWebRequest;

				// Get the response.
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						// Create the serializer.
						DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

						// Create the object.
						return serializer.ReadObject(response.GetResponseStream()) as T;
					}
				}
			}
			catch { }

			// Else, return the default object.
			return default(T);
		}

		#endregion
	}
}
