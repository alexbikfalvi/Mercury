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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MercuryApi.Api
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
			return LocalInformation.Parse(MercuryService.Get(new Uri(MercuryService.urlLocalInformation)));
		}

		#endregion

		#region Private methods

		public static JObject Get(Uri uri)
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
						using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
						{
							using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
							{
								return JObject.Load(jsonReader);
							}
						}
					}
				}
			}
			catch { }

			// Else, return null.
			return null;
		}

		#endregion
	}
}
