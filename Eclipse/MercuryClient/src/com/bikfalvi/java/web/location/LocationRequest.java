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

import javax.xml.parsers.ParserConfigurationException;

import org.xml.sax.SAXException;

import com.bikfalvi.java.async.AsyncResult;
import com.bikfalvi.java.web.WebCallback;
import com.bikfalvi.java.web.WebRequest;
import com.bikfalvi.java.web.WebResult;

/**
 * A class representing a location request.
 * @author Alex
 *
 */
public class LocationRequest extends WebRequest {
	private static final String url = "http://freegeoip.net/xml/%s";
	
	/**
	 * Executes a web request for the location information of current IP address.
	 * @return The web result.
	 * @throws IOException 
	 * @throws SAXException 
	 * @throws ParserConfigurationException 
	 */
	public LocationResult executeLocationRequest() throws IOException, ParserConfigurationException, SAXException {
		// Execute the request.
		WebResult webResult = super.execute(String.format(LocationRequest.url, ""));
		// Parse the received data.
		return LocationResult.parse(webResult.getResponseStream()); 
	}

	/**
	 * Begins an asynchronous web request for the location information of the current IP address.
	 * @param callback The callback method.
	 * @return The result of the asynchronous operation.
	 * @throws IOException 
	 */
	public AsyncResult beginLocationRequest(WebCallback callback) throws IOException {
		// Begin the request.
		return super.begin(String.format(LocationRequest.url, ""), callback);
	}
	
	/**
	 * Ends an asynchronous web request for the location information.
	 * @return The location result.
	 * @throws IOException 
	 * @throws SAXException 
	 * @throws ParserConfigurationException 
	 */
	public LocationResult endLocationRequest(WebResult result) throws IOException, ParserConfigurationException, SAXException {
		// Get the web result.
		WebResult webResult = super.end(result);
		// Parse the received data.
		return LocationResult.parse(webResult.getResponseStream());
	}
}
