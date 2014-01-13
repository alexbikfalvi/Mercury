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

namespace Mercury.Net.Core
{
	/// <summary>
	/// A class representing the traceroute settings.
	/// </summary>
	public sealed class TracerouteSettings
	{
		private readonly object sync = new object();

		/// <summary>
		/// Creates a traceroute settings instance with the default values.
		/// </summary>
		public TracerouteSettings()
		{
			this.MaximumHops = 30;
			this.MaximumAttempts = 3;
			this.MaximumFailedHops = 10;
			this.StopHopOnSuccess = true;
			this.StopTracerouteOnFail = true;
			this.DataLength = 32;
			this.Timeout = 1000;
			this.DontFragment = false;
		}

		// Public properties.

		/// <summary>
		/// Gets the synchronization object.
		/// </summary>
		public object Sync { get { return this.sync; } }
		/// <summary>
		/// Gets or sets the maximum number of hops for the traceroute.
		/// </summary>
		public byte MaximumHops { get; set; }
		/// <summary>
		/// Gets or sets the maximum number of attempts per hop.
		/// </summary>
		public byte MaximumAttempts { get; set; }
		/// <summary>
		/// Gets or sets the maximum number of failed consecutive hops.
		/// </summary>
		public byte MaximumFailedHops { get; set; }
		/// <summary>
		/// Gets or sets whether the traceroute for a hop stops after the first success.
		/// </summary>
		public bool StopHopOnSuccess { get; set; }
		/// <summary>
		/// Gets or sets whether the traceroute stops after the maximum number of consecututive failed hops was reached.
		/// </summary>
		public bool StopTracerouteOnFail { get; set; }
		/// <summary>
		/// Gets or sets the traceroute message data length.
		/// </summary>
		public int DataLength { get; set; }
		/// <summary>
		/// Gets or sets the traceroute message timeout.
		/// </summary>
		public int Timeout { get; set; }
		/// <summary>
		/// Gets or sets whether the IP packets set the DF bit.
		/// </summary>
		public bool DontFragment { get; set; }
	}
}
