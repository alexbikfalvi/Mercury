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

import java.net.InetAddress;

/// <summary>
/// A class representing a traceroute hop result.
/// </summary>
public class TracerouteHop
{
	private final int ttl;
	private final InetAddress address;

	/**
	 * Creates a new traceroute hop result instance.
	 * @param ttl The time-to-live.
	 * @param address The hop Internet address.
	 */
	public TracerouteHop(int ttl, InetAddress address)
	{
		this.ttl = ttl;
		this.address = address;
	}

	/**
	 * Gets the time-to-live for this hop.
	 * @return The time-to-live.
	 */
	public int getTimeToLive() {
		return this.ttl;
	}
	
	/**
	 * Gets the IP address for this hop.
	 * @return The IP address.
	 */
	public InetAddress getAddress() {
		return this.address;
	}
}

