/** 
 * Copyright (C) 2013 Alex Bikfalvi
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

package com.bikfalvi.java.web.location;

import java.io.IOException;
import java.io.InputStream;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.xml.sax.SAXException;

import com.bikfalvi.java.xml.XmlExtensions;

/**
 * A class representing the result of a geo-location request.
 * @author Alex Bikfalvi
 *
 */
public final class LocationResult
{
	private String address;
	private String countryCode;
	private String countryName;
	private String regionCode;
	private String regionName;
	private String city;
	private String zipCode;
	private String latitude;
	private String longitude;
	private String metroCode;
	private String areaCode;
	
	/**
	 * Creates a new location result instance.
	 */
	private LocationResult()
	{
	}
	
	/**
	 * Parses an XML String into a location result object.
	 * @param stream The XML string.
	 * @return The location result.
	 * @throws ParserConfigurationException 
	 * @throws IOException 
	 * @throws SAXException 
	 */
	public static LocationResult parse(InputStream stream) throws ParserConfigurationException, SAXException, IOException
	{
		// Create the document builder factory.
		DocumentBuilderFactory domFactory = DocumentBuilderFactory.newInstance();
		// Create the document builder.
		DocumentBuilder domBuilder = domFactory.newDocumentBuilder();
		
		// Parse the XML document from the input string.
		Document document = domBuilder.parse(stream);

		// Create the location result object.
		LocationResult result = new LocationResult();

		// Set the properties.
		result.address = XmlExtensions.getElementValue(document.getDocumentElement(), "Ip");
		result.countryCode = XmlExtensions.getElementValue(document.getDocumentElement(), "CountryCode");
		result.countryName = XmlExtensions.getElementValue(document.getDocumentElement(), "CountryName");
		result.regionCode = XmlExtensions.getElementValue(document.getDocumentElement(), "RegionCode");
		result.regionName = XmlExtensions.getElementValue(document.getDocumentElement(), "RegionName");
		result.city = XmlExtensions.getElementValue(document.getDocumentElement(), "City");
		result.zipCode = XmlExtensions.getElementValue(document.getDocumentElement(), "ZipCode");
		result.latitude = XmlExtensions.getElementValue(document.getDocumentElement(), "Latitude");
		result.longitude = XmlExtensions.getElementValue(document.getDocumentElement(), "Longitude");
		result.metroCode = XmlExtensions.getElementValue(document.getDocumentElement(), "MetroCode");
		result.areaCode = XmlExtensions.getElementValue(document.getDocumentElement(), "AreaCode");

		// Return the result.
		return result;
	}

	/**
	 * Gets the IP address.
	 * @return The IP address.
	 */
	public String getAddress() {
		return address;
	}

	/**
	 * Gets the country code.
	 * @return The country code.
	 */
	public String getCountryCode() {
		return countryCode;
	}

	/**
	 * Gets the country name.
	 * @return The country name.
	 */
	public String getCountryName() {
		return countryName;
	}

	/**
	 * Gets the region code.
	 * @return The region code.
	 */
	public String getRegionCode() {
		return regionCode;
	}

	/**
	 * Gets the region name.
	 * @return The region name.
	 */
	public String getRegionName() {
		return regionName;
	}

	/**
	 * Gets the city.
	 * @return The city.
	 */
	public String getCity() {
		return city;
	}

	/**
	 * Gets the ZIP code.
	 * @return The ZIP code.
	 */
	public String getZipCode() {
		return zipCode;
	}

	/**
	 * Gets the latitude.
	 * @return The latitude.
	 */
	public String getLatitude() {
		return latitude;
	}

	/**
	 * Gets the longitude.
	 * @return The longitude.
	 */
	public String getLongitude() {
		return longitude;
	}

	/**
	 * Gets the metro code.
	 * @return The metro code.
	 */
	public String getMetroCode() {
		return metroCode;
	}

	/**
	 * Gets the area code.
	 * @return The area code.
	 */
	public String getAreaCode() {
		return areaCode;
	}
}
