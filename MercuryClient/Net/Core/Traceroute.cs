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
using Mercury.Threading;

namespace Mercury.Net.Core
{
	/// <summary>
	/// A class representing a traceroute for the Internet Protocol.
	/// </summary>
	public sealed class Traceroute : IDisposable
	{
		private readonly TracerouteSettings settings;
		private readonly Ping ping = new Ping();

		/// <summary>
		/// Creates a new traceroute instance.
		/// </summary>
		/// <param name="settings">The traceroute settings.</param>
		public Traceroute(TracerouteSettings settings)
		{
			this.settings = settings;
		}

		// Public methods.

		/// <summary>
		/// Disposes the current object.
		/// </summary>
		public void Dispose()
		{
			// Dispose the ping object.
			this.ping.Dispose();
			// Suppress the finalizer.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Runs a traceroute to the specified destination.
		/// </summary>
		/// <param name="destination">The destination.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The result of the traceroute operation.</returns>
		public TracerouteResult Run(IPAddress destination, CancellationToken cancellationToken)
		{
			// Create the traceroute result.
			TracerouteResult result = new TracerouteResult(destination);

			// Create the data.
			byte[] data = new byte[this.settings.DataLength];

			// The completed flag.
			bool completed = false;
			// The hop fail count.
			int fail = 0;

			// For each time-to-live.
			for (int ttl = 1; (ttl < this.settings.MaximumHops) && (!completed); ttl++)
			{
				bool success = false;

				// Create a new traceroute hop result for this time-to-live.
				TracerouteHopResult hopResult = new TracerouteHopResult(ttl);
				result.Add(hopResult);

				for (int attempt = 0; attempt < this.settings.MaximumAttempts; attempt++)
				{
					// If a cancellation is requested, return null.
					if (cancellationToken.IsCanceled) return null;

					try
					{
						// Send a ping request.
						PingReply reply = this.ping.Send(destination, this.settings.Timeout, data, new PingOptions(ttl, this.settings.DontFragment));
						// Add the reply to the current hop result.
						success = success || hopResult.AddReply(reply);
						
						// If the hop was successful and the stop on hop success flag is set, stop the hop loop.
						if (this.settings.StopHopOnSuccess && success)
						{
							attempt = this.settings.MaximumAttempts;
						}

						if (reply.Status == IPStatus.Success)
						{
							completed = true;
						}
					}
					catch { }
				}

				// If the hop was successful reset the fail count, else increment the hop count.
				fail = success ? 0 : fail + 1;

				// If the stop traceroute on fail flag is set, and if the maximum number of hops was reached.
				if (this.settings.StopTracerouteOnFail && (fail >= this.settings.MaximumFailedHops))
				{
					ttl = this.settings.MaximumHops;
				}
			}

			// Return the result.
			return result;
		}
	}
}
