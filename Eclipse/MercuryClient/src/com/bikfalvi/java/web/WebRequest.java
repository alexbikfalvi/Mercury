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

import com.bikfalvi.java.async.AsyncResult;
import com.bikfalvi.java.threading.ThreadPool;

/**
 * A class representing a web request.
 * @author Alex Bikfalvi
 *
 */
public class WebRequest {
	/**
	 * Executes a web request for the specified URL. 
	 * @param url The URL.
	 * @return The result of the web operation.
	 * @throws IOException 
	 */
	public WebResult execute(String url) throws IOException {
		// Create the request state.
		WebState state = new WebState(url);
		
		// Execute the get request for the current state.
		state.execute();
		
		// Return the result.
		return state;
	}
	
	/**
	 * Begins an asynchronous request for the specified URL.
	 * @param url The URL.
	 * @param callback The callback of the asynchronous operation.
	 * @return The result of the asynchronous operation.
	 * @throws IOException 
	 */
	public AsyncResult begin(final String url, final WebCallback callback) throws IOException {
		// Create the request state.
		final WebState state = new WebState(url);
		
		// Execute the request on the thread pool.
		ThreadPool.execute(new Runnable() {
			@Override
			public void run() {
				try {
					// Execute the get request for the current state.
					state.execute();
				}
				catch (IOException exception) {
					state.setException(exception);
				}
				finally {
					if (null != callback) callback.callback(state);
				}
			}
		});
		// Return the request state.
		return state;
	}
	
	/**
	 * Begins an asynchronous request for the specified state.
	 * @param state The web state.
	 * @param callback The callback of the asynchronous operation.
	 * @return The result of the asynchronous operation.
	 * @throws IOException 
	 */
	public AsyncResult begin(final WebState state, final WebCallback callback) {
		
		// Execute the request on the thread pool.
		ThreadPool.execute(new Runnable() {
			@Override
			public void run() {
				try {
					// Execute the get request for the current state.
					state.execute();
				}
				catch (IOException exception) {
					state.setException(exception);
				}
				finally {
					if (null != callback) callback.callback(state);
				}
			}
		});
		// Return the request state.
		return state;		
	}
	
	/**
	 * Ends an asynchronous web operation.
	 * @param result The result of the web operation.
	 * @return The result of the web operation.
	 * @throws IOException 
	 */
	public WebResult end(WebResult result) throws IOException {
		// If the web operation is not null. 
		if (null != result.getException()) {
			// Throw the exception.
			throw result.getException();
		}
		// Else, return the result.
		return result;
	}
}
