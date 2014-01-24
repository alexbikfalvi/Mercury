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

package com.bikfalvi.java.net.core;

/**
 * A class representing the traceroute settings.
 */
public final class TracerouteSettings
{
	private byte maximumHops;
	private int timeout;
	
	/// <summary>
	/// Creates a traceroute settings instance with the default values.
	/// </summary>
	public TracerouteSettings()
	{
		this.maximumHops = 30;
		this.timeout = 1000;
	}

	/**
	 * Gets the maximum hops for the traceroute.
	 * @return The maximum hops.
	 */
	public byte getMaximumHops() {
		return this.maximumHops;
	}

	/**
	 * Sets the maximum hops for the traceroute.
	 * @param maximumHops The maximum hops.
	 */
	public void setMaximumHops(byte maximumHops) {
		this.maximumHops = maximumHops;
	}	
	
	/**
	 * Gets the timeout for a traceroute hop.
	 * @return The timeout in milliseconds.
	 */
	public int getTimeout() {
		return this.timeout;
	}

	/**
	 * Sets the timeout for a traceroute hop/
	 * @param timeout The timeout in milliseconds.
	 */
	public void setTimeout(int timeout) {
		this.timeout = timeout;
	}
}
