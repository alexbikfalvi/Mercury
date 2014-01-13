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

namespace Mercury.Threading
{
	/// <summary>
	/// A class representing a cancellation token.
	/// </summary>
	public sealed class CancellationToken
	{
		private volatile bool canceled = false;
		private readonly object sync = new object();

		/// <summary>
		/// Creates a new cancellation token instance.
		/// </summary>
		internal CancellationToken()
		{
		}

		// Public properties.

		/// <summary>
		/// Gets whether there was a cancellation requested.
		/// </summary>
		public bool IsCanceled { get { lock (this.sync) { return this.canceled; } } }

		// Internal method.

		/// <summary>
		/// Resets the cancellation token.
		/// </summary>
		public void Reset()
		{
			lock (this.sync) { this.canceled = false; }
		}

		/// <summary>
		/// Cancels the token.
		/// </summary>
		public void Cancel()
		{
			lock (this.sync) { this.canceled = true; }
		}
	}
}
