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
using System.ComponentModel;
using System.Runtime;
using System.Windows.Forms;
using System.Threading;

namespace DotNetApi.Windows.Forms
{
	/// <summary>
	/// An parameterless action delegate.
	/// </summary>
	public delegate void Action();

	/// <summary>
	/// A base class for thread-safe forms.
	/// </summary>
	public class ThreadSafeForm : Form
	{
		private readonly ManualResetEvent eventHandleCreated = new ManualResetEvent(false);
		private readonly Action<Action> action;

		/// <summary>
		/// Creates a new thread-safe control instance.
		/// </summary>
		public ThreadSafeForm()
		{
			// Set the action.
			this.action = new Action<Action>(this.Invoke);
		}

		// Public methods.

		/// <summary>
		/// Executes the specified action, on the thread that owns the control's underlying window handle, with the specified list of arguments.
		/// </summary>
		/// <param name="action">The action.</param>
		public void Invoke(Action action)
		{
			// If the method is called on a different thread.
			if (this.InvokeRequired)
			{
				// Invoke the action delegate.
				this.Invoke(this.action, new object[] { action });
			}
			else if (!this.IsDisposed)
			{
				// Call the action.
				action();
			}
		}

		// Protected methods.

		/// <summary>
		/// Waits for the window handle of the current control to be created.
		/// </summary>
		/// <returns><b>True</b> if the wait was successful, <b>false</b> if the method exited because the control was disposed.</returns>
		protected bool WaitForHandle()
		{
			// Wait for the control handle to be created.
			while (!this.IsHandleCreated)
			{
				// If the control is disposed, return false.
				if (this.IsDisposed) return false;
				// Wait for the control handle to be created.
				this.eventHandleCreated.WaitOne();
			}
			return true;
		}

		/// <summary>
		/// An event handler called when the object is being disposed.
		/// </summary>
		/// <param name="disposed">If <b>true</b>, clean both managed and native resources. If <b>false</b>, clean only native resources.</param>
		protected override void Dispose(bool disposing)
		{
			// Dispose the current resources.
			if (disposing)
			{
				this.eventHandleCreated.Close();
			}
			// Call the base class method.
			base.Dispose(disposing);
		}

		/// <summary>
		/// An event handler called when the handle of the current control was created.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnHandleCreated(EventArgs e)
		{
			// Call the base class method.
			base.OnHandleCreated(e);
			// Set the event.
			this.eventHandleCreated.Set();
		}
	}
}
