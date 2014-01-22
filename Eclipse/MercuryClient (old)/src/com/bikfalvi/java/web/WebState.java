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

package com.bikfalvi.java.web;

import java.io.IOException;
import java.net.HttpURLConnection;

/**
 * A class representing the state of a web operation.
 * @author Alex Bikfalvi
 *
 */
public class WebState extends WebResult {
	private final HttpURLConnection connection;
	
	/**
	 * Creates a new web state instance using the specified URL.
	 * @param url The web address.
	 * @throws IOException 
	 */
	public WebState(String url) throws IOException {
		super(url);
		// Open the web connection.
		this.connection = (HttpURLConnection) this.getUrl().openConnection();
	}
	
	/**
	 * Creates a GET web request for the current URL.
	 * @throws IOException 
	 */
	public void get() throws IOException {
		// Execute the request and set the response code.
		int code = this.connection.getResponseCode();

		// Get the response data.
		this.setResponse(code, this.connection.getInputStream());
		
		// Complete the request.
		this.complete();
	}
}
