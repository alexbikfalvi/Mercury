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
using System.Net;
using System.Threading;

namespace Mercury.Net.Core
{
	/// <summary>
	/// A class representing the multipath traceroute settings.
	/// </summary>
	public sealed class MultipathTracerouteSettings
	{
		/// <summary>
		/// Creates a new instance of the multipath traceroute settings.
		/// </summary>
		public MultipathTracerouteSettings()
		{
		}

		// Public methods.

		/// <summary>
		/// Runs a multipath traceroute to the specified destination.
		/// </summary>
		/// <param name="destination">The destination.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The result of the traceroute operation.</returns>
		public MultipathTracerouteResult Run(IPAddress destination, CancellationToken cancel)
		{
			return null;
		}
	}
}
