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
using System.Net.NetworkInformation;

namespace Mercury.Net.Core
{
	/// <summary>
	/// A class representing a traceroute hop result.
	/// </summary>
	public class TracerouteHopResult
	{
		private readonly int ttl;
		private IPAddress address;
		private int count = 0;
		private long rttMin = long.MaxValue;
		private long rttMax = long.MinValue;
		private long rttSum = 0;

		/// <summary>
		/// Creates a new traceroute hop result instance.
		/// </summary>
		/// <param name="ttl">The time-to-live.</param>
		public TracerouteHopResult(int ttl)
		{
			this.ttl = ttl;
		}

		// Public properties.

		/// <summary>
		/// Gets the time-to-live for this hop.
		/// </summary>
		public int TimeToLive { get { return this.ttl; } }
		/// <summary>
		/// Gets the IP address for this hop.
		/// </summary>
		public IPAddress Address { get { return this.address; } }
		/// <summary>
		/// Gets the number of replies received for this hop.
		/// </summary>
		public int Count { get { return this.count; } }
		/// <summary>
		/// Gets the minimum round-trip time.
		/// </summary>
		public long MinimumRoundtripTime { get { return this.rttMin; } }
		/// <summary>
		/// Gets the maximum round-trip time.
		/// </summary>
		public long MaximumRoundtripTime { get { return this.rttMax; } }
		/// <summary>
		/// Gets the average round-trip time.
		/// </summary>
		public double AverageRoundtripTime { get { return this.count > 0 ? (double)this.rttSum / this.count : 0; } }

		// Public methods.

		/// <summary>
		/// Adds a new reply to this traceroute hop.
		/// </summary>
		/// <param name="reply">The reply.</param>
		/// <returns><b>True</b> if the reply was successful, or <b>false</b> otherwise.</returns>
		public bool AddReply(PingReply reply)
		{
			// If the reply status is success or time-exceeded.
			if ((reply.Status == IPStatus.Success) || (reply.Status == IPStatus.TtlExpired))
			{
				// Set the IP address.
				if (null == this.address)
				{
					this.address = reply.Address;
				}
				else if (reply.Address != this.address) return false;

				// Compute the round-trip time.
				if (this.rttMin > reply.RoundtripTime) this.rttMin = reply.RoundtripTime;
				if (this.rttMax < reply.RoundtripTime) this.rttMax = reply.RoundtripTime;
				this.rttSum += reply.RoundtripTime;
				this.count++;

				return true;
			}
			return false;
		}
	}
}
