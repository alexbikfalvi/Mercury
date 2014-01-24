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
import java.util.ArrayList;

/**
 * A class representing the result of a traceroute operation.
 * @author Alex Bikfalvi
 *
 */
public class TracerouteResult
{
	private final InetAddress destination;
	private final ArrayList<TracerouteHop> hops = new ArrayList<TracerouteHop>();

	/**
	 * Creates a new traceroute result instance.
	 * @param destination The traceroute destination.
	 */
	public TracerouteResult(InetAddress destination)
	{
		this.destination = destination;
	}

	/**
	 * destination
	 * @return The destination.
	 */
	public InetAddress getDestination() {
		return this.destination;
	}

	/**
	 * Gets the list of hops.
	 * @return The list.
	 */
	public Iterable<TracerouteHop> getHops() {
		return this.hops;
	}

	/**
	 * Adds a new hop result to the traceroute result.
	 * @param hop The hop result.
	 */
	public void add(TracerouteHop hop)
	{
		this.hops.add(hop);
	}
}
