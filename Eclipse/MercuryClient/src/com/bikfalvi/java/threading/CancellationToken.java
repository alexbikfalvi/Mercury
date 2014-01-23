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

package com.bikfalvi.java.threading;

/**
 * A class representing a cancellation token.
 * @author Alex Bikfalvi
 *
 */
public final class CancellationToken
{
	private volatile boolean canceled = false;
	private final Object sync = new Object();

	/**
	 * Creates a new cancellation token instance.
	 */
	public CancellationToken()
	{
	}

	/**
	 * Gets whether there was a cancellation requested.
	 * @return True if a cancellation was requested, false otherwise.
	 */
	public boolean isCanceled() {
		synchronized (this.sync) {
			return this.canceled;
		}
	}

	/**
	 * Resets the cancellation token.
	 */
	public void reset()
	{
		synchronized (this.sync) {
			this.canceled = false;
		}
	}

	/**
	 * Cancels the token.
	 */
	public void cancel()
	{
		synchronized (this.sync) {
			this.canceled = true;
		}
	}
}
