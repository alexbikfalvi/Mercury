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
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Mercury.Net.Core
{
	/// <summary>
	/// A class representing the result of a traceroute operation.
	/// </summary>
	public class TracerouteResult
	{
		private readonly List<TracerouteHop> hops = new List<TracerouteHop>();

		/// <summary>
		/// Creates a new traceroute result instance.
		/// </summary>
		/// <param name="destination">The traceroute destination.</param>
		internal TracerouteResult(IPAddress destination)
		{
			this.Destination = destination;
		}

		// Public properties.

		/// <summary>
		/// Gets the destination.
		/// </summary>
		public IPAddress Destination { get; private set; }
		/// <summary>
		/// Gets the list of hops.
		/// </summary>
		public ICollection<TracerouteHop> Hops { get { return this.hops; } }

		// Internal methods.

		/// <summary>
		/// Adds a new hop result to the traceroute result.
		/// </summary>
		/// <param name="hopResult">The hop result.</param>
		internal void Add(TracerouteHop hopResult)
		{
			this.hops.Add(hopResult);
		}
	}
}
