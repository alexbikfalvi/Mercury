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

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.MalformedURLException;
import java.net.URL;

import org.apache.commons.io.IOUtils;

import com.bikfalvi.java.async.AsyncResult;

/**
 * A class representing the result of a web request.
 * @author Alex Bikfalvi.
 *
 */
public class WebResult implements AsyncResult {
	private final URL url;
	
	private Object sync = new Object();
	
	private int responseCode;

	private String responseEncoding = null;
	private byte[] responseData = null;
	private InputStream responseStream = null;
	private IOException exception = null;
	private boolean completed = false;

	/**
	 * Creates a new web result instance using the specified URL.
	 * @param url The web address.
	 * @throws MalformedURLException 
	 */
	protected WebResult(String url) throws MalformedURLException {
		// Create the URL.
		this.url = new URL(url);
	}
	
	@Override
	public boolean isCompleted() {
		synchronized (this.sync) {
			return this.completed;
		}
	}	
	
	/**
	 * Gets the web request URL.
	 * @return The URL.
	 */
	public URL getUrl() {
		return this.url;
	}
	
	/**
	 * Gets the result response code.
	 * @return The response code.
	 */
	public int getResponseCode() {
		return this.responseCode;
	}
	
	/**
	 * Gets the result response encoding.
	 * @return
	 */
	public String getResponseEncoding() {
		return responseEncoding;
	}

	/**
	 * Gets the result response data.
	 * @return The response data.
	 */
	public byte[] getResponseData() {
		return this.responseData;
	}
	
	/**
	 * Gets the result response data as a string.
	 * @return The response data string.
	 */
	public String getResponseDataAsString() {
		try {
			return this.responseData != null ? this.responseEncoding != null ? IOUtils.toString(this.responseData, this.responseEncoding) : IOUtils.toString(this.responseData, "UTF-8"): "";
		} catch (IOException e) {
			return "";
		}
	}
	
	/**
	 * Gets the result response stream.
	 * @return The response stream.
	 */
	public InputStream getResponseStream() {
		return this.responseStream;
	}	
	
	/**
	 * Gets the result exception.
	 * @return The exception.
	 */
	public IOException getException() {
		return this.exception;
	}
	
	/**
	 * Completes the web operation.
	 */
	protected void complete() {
		synchronized (this.sync) {
			this.completed = true;
		}
	}

	/**
	 * Sets the response.
	 * @param code The response code.
	 * @param stream The response input stream.
	 * @param encoding The response encoding.
	 * @throws IOException 
	 */
	protected void setResponse(int code, InputStream stream, String encoding) throws IOException {
		this.responseCode = code;
		this.responseEncoding = encoding;
		this.responseData = IOUtils.toByteArray(stream);
		this.responseStream = new ByteArrayInputStream(this.responseData);
	}
	
	/**
	 * Sets the result exception.
	 * @param exception The result exception. 
	 */
	protected void setException(IOException exception) {
		this.exception = exception;
	}
}
