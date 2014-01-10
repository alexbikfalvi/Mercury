/* 
 * Copyright (C) 2012-2013 Alex Bikfalvi
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
using System.Threading;

namespace Mercury.Async
{
	/// <summary>
	/// A class representing the state for a database asynchronous operation.
	/// </summary>
	public class AsyncResult : IAsyncResult, IDisposable
	{
		private object state;
		private ManualResetEvent wait = new ManualResetEvent(false);
		private bool completedSynchronously = false;
		private bool completed = false;

		/// <summary>
		/// Creates a new instance of the asynchronous state.
		/// </summary>
		/// <param name="state">The user state.</param>
		public AsyncResult(object state)
		{
			// Save the user state.
			this.state = state;
		}

		// Public properties.

		/// <summary>
		/// Returns the asynchronous user state.
		/// </summary>
		public object AsyncState { get { return this.state; } }

		/// <summary>
		/// Returns the wait handle.
		/// </summary>
		public WaitHandle AsyncWaitHandle { get { return this.wait; } }

		/// <summary>
		/// Indicates whether the operation completed synchronously.
		/// </summary>
		public bool CompletedSynchronously
		{
			get { return this.completedSynchronously; }
			set { this.completedSynchronously = value; }
		}
		/// <summary>
		/// Indicates whether the operation completed.
		/// </summary>
		public bool IsCompleted { get { return this.completed; } }

		// Public methods.

		/// <summary>
		/// Disposes the current request.
		/// </summary>
		public void Dispose()
		{
			// Call the event handler.
			this.Dispose(true);
			// Supress the finalizer.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Completes the asynchronous operation by setting the completed property and the wait handle
		/// to <b>true</b>.
		/// </summary>
		public void Complete()
		{
			this.completed = true;
			this.wait.Set();
		}

		// Protected methods.

		/// <summary>
		/// An event handler called when the object is being disposed.
		/// </summary>
		/// <param name="disposing">If <b>true</b>, clean both managed and native resources. If <b>false</b>, clean only native resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Signal the wait event.
				this.wait.Set();
				// Close the wait event.
				this.wait.Close();
			}
		}
	}
}
