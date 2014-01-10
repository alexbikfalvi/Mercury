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
using System.IO;
using System.Net;
using System.Threading;
using Mercury.Async;

namespace Mercury.Web
{
	/// <summary>
	/// A class representing the asynchronous request state.
	/// </summary>
	public sealed class AsyncWebResult : AsyncResult
	{
		public const int BUFFER_SIZE = 4096;
		private byte[] buffer = null;

		/// <summary>
		/// Constructs an object for an asynchronous request state.
		/// </summary>
		/// <param name="uri">The URI of the asynchronous request.</param>
		/// <param name="callback">The callback function for the asynchronous request.</param>
		/// <param name="userState">The user state.</param>
		public AsyncWebResult(Uri uri, AsyncWebRequestCallback callback, object userState = null)
			: base(userState)
		{
			this.SendData = new AsyncWebBuffer();
			this.ReceiveData = new AsyncWebBuffer();
			this.Request = (HttpWebRequest)WebRequest.Create(uri);
			this.Callback = callback;
			this.buffer = new byte[BUFFER_SIZE];
		}

		/// <summary>
		/// Gets or sets the exception of the asynchronous result.
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// The data sent by the asynchronous request.
		/// </summary>
		public AsyncWebBuffer SendData { get; set; }

		/// <summary>
		/// The data received by the asynchrounous request.
		/// </summary>
		public AsyncWebBuffer ReceiveData { get; set; }

		/// <summary>
		/// The request object corresponding to the asynchrounous operation.
		/// </summary>
		public HttpWebRequest Request { get; set; }

		/// <summary>
		/// The response object corresponding to the asynchronous request.
		/// </summary>
		public HttpWebResponse Response { get; set; }

		/// <summary>
		/// The stream object corresponding to the asynchronous request.
		/// </summary>
		public Stream Stream { get; set; }

		/// <summary>
		/// The buffer used to store the data returned by the asynchronous request.
		/// </summary>
		public byte[] Buffer { get { return this.buffer; } }

		/// <summary>
		/// The callback function for the asynchronous operation.
		/// </summary>
		public AsyncWebRequestCallback Callback { get; set; }
	}
}
