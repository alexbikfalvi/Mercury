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
using System.IO;
using System.Xml;
using Mercury.Xml;

namespace Mercury.Web.Location
{
	/// <summary>
	/// A class representing the result of a geo-location request.
	/// </summary>
	public sealed class LocationResult
	{
		/// <summary>
		/// Creates a new location result instance.
		/// </summary>
		private LocationResult()
		{
		}

		// Public methods.

		/// <summary>
		/// Gets the IP address.
		/// </summary>
		public string Address { get; private set; }
		/// <summary>
		/// Gets the country code.
		/// </summary>
		public string CountryCode { get; private set; }
		/// <summary>
		/// Gets the country name.
		/// </summary>
		public string CountryName { get; private set; }
		/// <summary>
		/// Gets the region code.
		/// </summary>
		public string RegionCode { get; private set; }
		/// <summary>
		/// Gets the region name.
		/// </summary>
		public string RegionName { get; private set; }
		/// <summary>
		/// Gets the city.
		/// </summary>
		public string City { get; private set; }
		/// <summary>
		/// Gets the ZIP code.
		/// </summary>
		public string ZipCode { get; private set; }
		/// <summary>
		/// Gets the latitude.
		/// </summary>
		public string Latitude { get; private set; }
		/// <summary>
		/// Gets the longitude.
		/// </summary>
		public string Longitude { get; private set; }
		/// <summary>
		/// Gets the metropolitan code.
		/// </summary>
		public string MetroCode { get; private set; }
		/// <summary>
		/// Gets the area code.
		/// </summary>
		public string AreaCode { get; private set; }

		// Public methods.

		/// <summary>
		/// Parses an XML string into a location result object.
		/// </summary>
		/// <param name="xml">The XML string.</param>
		/// <returns>The location result.</returns>
		public static LocationResult Parse(string xml)
		{
			// Create the XML document.
			XmlDocument document = new XmlDocument();

			// Create the location result object.
			LocationResult result = new LocationResult();

			using (StringReader reader = new StringReader(xml))
			{
				// Parse the string data into the XML document.
				document.Load(reader);
			}

			// Set the properties.
			result.Address = XmlExtensions.GetElementValue(document.DocumentElement, "Ip");
			result.CountryCode = XmlExtensions.GetElementValue(document.DocumentElement, "CountryCode");
			result.CountryName = XmlExtensions.GetElementValue(document.DocumentElement, "CountryName");
			result.RegionCode = XmlExtensions.GetElementValue(document.DocumentElement, "RegionCode");
			result.RegionName = XmlExtensions.GetElementValue(document.DocumentElement, "RegionName");
			result.City = XmlExtensions.GetElementValue(document.DocumentElement, "City");
			result.ZipCode = XmlExtensions.GetElementValue(document.DocumentElement, "ZipCode");
			result.Latitude = XmlExtensions.GetElementValue(document.DocumentElement, "Latitude");
			result.Longitude = XmlExtensions.GetElementValue(document.DocumentElement, "Longitude");
			result.MetroCode = XmlExtensions.GetElementValue(document.DocumentElement, "MetroCode");
			result.AreaCode = XmlExtensions.GetElementValue(document.DocumentElement, "AreaCode");

			// Return the result.
			return result;
		}

		// Private methods.

		private static XmlElement GetElement(XmlElement element, string name)
		{
			// Get the child elements.
			XmlNodeList children = element.GetElementsByTagName(name);
			// If the list is not empty, return the first element.
			return children.Count > 0 ? children[0] as XmlElement : null;
		}
	}
}
