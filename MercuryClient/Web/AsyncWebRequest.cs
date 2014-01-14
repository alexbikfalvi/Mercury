/* 
 * Copyright (C) 2012 Alex Bikfalvi
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
using System.Text;
using Mercury;

namespace Mercury.Web
{
	/// <summary>
	/// An interface for conversion of the asynchronous operation data to a custom type.
	/// </summary>
	/// <typeparam name="T">The custom type used for conversion.</typeparam>
	public interface IAsyncFunction<T>
	{
		/// <summary>
		/// Processes the string data from the asynchronous operation, and returns a custom type result.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>The custom type result.</returns>
		T GetResult(string data);
	}

	/// <summary>
	/// A delegate for conversion of the asynchronous operation data to a custom type.
	/// </summary>
	/// <typeparam name="T">The custom type used for conversion.</typeparam>
	/// <param name="data">The data.</param>
	/// <returns>The custom type result.</returns>
	public delegate T IAsyncDelegate<T>(string data);

	/// <summary>
	/// A delegate representing the callback from an asynchronous web request.
	/// </summary>
	/// <param name="result">The result of the asynchronous web request.</param>
	public delegate void AsyncWebRequestCallback(AsyncWebResult result);

	/// <summary>
	/// A class that represents an asynchronous web request.
	/// </summary>
	public class AsyncWebRequest
	{
		public static TimeSpan defaultTimeout = new TimeSpan(0, 0, 10);

		/// <summary>
		/// Constructor of an asynchronous request.
		/// </summary>
		public AsyncWebRequest()
		{
			this.Timeout = AsyncWebRequest.defaultTimeout;
		}

		/// <summary>
		/// The timeout for the asynchrnous operation, when executed synchronously.
		/// </summary>
		public TimeSpan Timeout { get; set; }

		/// <summary>
		/// Creates the state of an asynchronous request for a web resource.
		/// </summary>
		/// <param name="uri">The URI of the web resource.</param>
		/// <param name="callback">The delegate to the callback function.</param>
		/// <param name="userState">The user state.</param>
		/// <returns>The state of the asynchronous request.</returns>
		public static AsyncWebResult Create(Uri uri, AsyncWebRequestCallback callback, object userState)
		{
			return new AsyncWebResult(uri, callback, userState);
		}

		/// <summary>
		/// Begins an asynchronous request for a web resource.
		/// </summary>
		/// <param name="uri">The URI of the web resource.</param>
		/// <param name="callback">The delegate to the callback function.</param>
		/// <param name="userState">The user state.</param>
		/// <returns>The result of the asynchronous request.</returns>
		public IAsyncResult Begin(Uri uri, AsyncWebRequestCallback callback, object userState)
		{
			return this.BeginAsyncRequest(new AsyncWebResult(uri, callback, userState));
		}

		/// <summary>
		/// Begins an asynchronous request for a web resource.
		/// </summary>
		/// <param name="asyncState">The state of the asynchronous request.</param>
		/// <returns>The result of the asynchronous request.</returns>
		public IAsyncResult Begin(AsyncWebResult asyncState)
		{
			return this.BeginAsyncRequest(asyncState);
		}

		/// <summary>
		/// Completes the asynchronous operation and returns the asynchronous web result.
		/// </summary>
		/// <param name="result">The asynchronous result.</param>
		/// <returns>The asynchronous web result.</returns>
		public AsyncWebResult End(IAsyncResult result)
		{
			// Get the state of the asynchronous operation.
			AsyncWebResult asyncState = (AsyncWebResult)result;

			// If an exception was thrown during the execution of the asynchronous operation.
			if (asyncState.Exception != null)
			{
				// Throw the exception.
				throw asyncState.Exception;
			}

			// Return the web result.
			return asyncState;
		}

		/// <summary>
		/// Completes the asynchronous operation and returns the asynchrnous web result and the received data,
		/// if any, as a string using the response encoding.
		/// </summary>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="data">The received data as a string using the response encoding.</param>
		/// <returns>The asynchronous web result.</returns>
		public AsyncWebResult End(IAsyncResult result, out string data)
		{
			// Get the state of the asynchronous operation.
			AsyncWebResult asyncState = (AsyncWebResult)result;

			// If the response is not null.
			if (asyncState.Response != null)
			{
				// Get the encoding
				Encoding encoding = asyncState.Response.CharacterSet != null ?
					Encoding.GetEncoding(asyncState.Response.CharacterSet) : Encoding.UTF8;

				// Get the string data.
				data = (null != asyncState.ReceiveData.Data) ? encoding.GetString(asyncState.ReceiveData.Data) : null;
			}
			else
			{
				// Set the data to null.
				data = null;
			}

			// If an exception was thrown during the execution of the asynchronous operation.
			if (asyncState.Exception != null)
			{
				// Throw the exception.
				throw asyncState.Exception;
			}

			// Return the web result.
			return asyncState;
		}

		/// <summary>
		/// Completes the asynchronous operation and returns the received data.
		/// </summary>
		/// <typeparam name="T">The type of the returned data.</typeparam>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="func">An instance used to convert the received data to the desired format.</param>
		/// <returns>The data received during the asynchronous operation.</returns>
		protected T End<T>(IAsyncResult result, IAsyncFunction<T> func)
		{
			return this.End<T>(result, func.GetResult);
		}

		/// <summary>
		/// Completes the asynchronous operation and returns the received data.
		/// </summary>
		/// <typeparam name="T">The type of the returned data.</typeparam>
		/// <param name="result">The asynchronous result.</param>
		/// <param name="func">A delegate method used to convert the received data to the desired format.</param>
		/// <returns>The data received during the asynchronous operation.</returns>
		protected T End<T>(IAsyncResult result, IAsyncDelegate<T> func)
		{
			// Get the state of the asynchronous operation.
			AsyncWebResult asyncState = (AsyncWebResult)result;

			// If an exception was thrown during the execution of the asynchronous operation.
			if (asyncState.Exception != null)
			{
				// Throw the exception.
				throw asyncState.Exception;
			}

			// Get the encoding
			Encoding encoding = string.IsNullOrEmpty(asyncState.Response.CharacterSet) ? Encoding.UTF8 : Encoding.GetEncoding(asyncState.Response.CharacterSet);

			// Get the string data.
			string data = (null != asyncState.ReceiveData.Data) ? encoding.GetString(asyncState.ReceiveData.Data) : null;

			// Return the converted data.
			return func(data);
		}

		/// <summary>
		/// Cancels the asynchronous web request.
		/// </summary>
		/// <param name="result">The result of the asynchronous operation to cancel.</param>
		public void Cancel(IAsyncResult result)
		{
			// Get the state of the asynchronous operation.
			AsyncWebResult asyncState = (AsyncWebResult)result;

			// Use the system thread pool to cancel the request on a worker thread.
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.CancelAsync), asyncState);
		}

		/// <summary>
		/// Cancels the asynchronous web request.
		/// </summary>
		/// <param name="state">The result of the asynchronous operation to cancel.</param>
		protected void CancelAsync(object state)
		{
			AsyncWebResult asyncState = state as AsyncWebResult;

			// Abort the web request.
			asyncState.Request.Abort();
		}

		/// <summary>
		/// Begins an asynchronous web request.
		/// </summary>
		/// <param name="asyncState">The asynchronous web state.</param>
		/// <returns>The result of the asynchronous operation.</returns>
		protected IAsyncResult BeginAsyncRequest(AsyncWebResult asyncState)
		{
			// Use the system thread pool to begin the asynchronous request on a worker thread.
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.BeginWebRequest), asyncState);

			return asyncState;
		}

		/// <summary>
		/// Begins a web request for the specified asynchronous state.
		/// </summary>
		/// <param name="state">The asynchronous state.</param>
		protected void BeginWebRequest(object state)
		{
			AsyncWebResult asyncState = state as AsyncWebResult;

			try
			{
				// If there is send data.
				if (asyncState.SendData.Data != null)
				{
					// Get the request stream.
					using (Stream stream = asyncState.Request.GetRequestStream())
					{
						// Write the send data to the stream.
						stream.Write(asyncState.SendData.Data, 0, asyncState.SendData.Data.Length);
					}
				}
			}
			catch (Exception exception)
			{
				// If an exception occurred while writing to the stream, set the exception.
				asyncState.Exception = exception;
				// Signal that the operation has completed.
				asyncState.Complete();
				// Call the callback function
				if (asyncState.Callback != null) asyncState.Callback(asyncState);
				// Dispose the asynchronous state.
				asyncState.Dispose();
				// Return.
				return;
			}

			// Begin the web request.
			asyncState.Request.BeginGetResponse(this.EndWebRequest, asyncState);
		}

		/// <summary>
		/// Ends a web request for the specified asynchronous operation result.
		/// </summary>
		/// <param name="result">The result of the asynchronous operation.</param>
		protected void EndWebRequest(IAsyncResult result)
		{
			// Get the state of the asynchronous operation
			AsyncWebResult asyncState = (AsyncWebResult)result.AsyncState;

			try
			{
				// If the asynchrounous operation is not completed
				if (!result.IsCompleted)
				{
					// Wait on the operation handle until the timeout expires
					if (!result.AsyncWaitHandle.WaitOne(this.Timeout))
					{
						// If no signal was received in the timeout period, throw a timeout exception.
						throw new TimeoutException(string.Format("The web request did not complete within the timeout {0}.", this.Timeout));
					}
				}

				// Complete the web request and get the result.
				try
				{
					// Try get the normal response.
					asyncState.Response = (HttpWebResponse)asyncState.Request.EndGetResponse(result);
				}
				catch (WebException exception)
				{
					// For a web exception, use the exception response.
					asyncState.Response = (HttpWebResponse)exception.Response;
					// Set the exception.
					asyncState.Exception = exception;
				}

				if (null != asyncState.Response)
				{
					// Get the stream corresponding to the web response.
					asyncState.Stream = asyncState.Response.GetResponseStream();
					// Begin reading for the returned data.
					this.BeginStreamRead(asyncState);
				}
				else
				{
					// Signal that the operation has completed.
					asyncState.Complete();
					// Call the callback function
					if (asyncState.Callback != null) asyncState.Callback(asyncState);
					// Dispose the asynchronous state.
					asyncState.Dispose();
				}
			}
			catch (Exception exception)
			{
				// Set the exception.
				asyncState.Exception = exception;
				// Signal that the operation has completed.
				asyncState.Complete();
				// Call the callback function
				if (asyncState.Callback != null) asyncState.Callback(asyncState);
				// Dispose the asynchronous state.
				asyncState.Dispose();
			}
		}

		/// <summary>
		/// Begins a read from the data stream of the specified asynchronous request.
		/// </summary>
		/// <param name="asyncState">The state of the asynchronous operation.</param>
		/// <returns>The result of the asynchronous read operation.</returns>
		protected IAsyncResult BeginStreamRead(AsyncWebResult asyncState)
		{
			// Begin the read operation.
			return asyncState.Stream.BeginRead(asyncState.Buffer, 0, AsyncWebResult.BUFFER_SIZE, this.EndStreamRead, asyncState);
		}

		/// <summary>
		/// Ends a read from the data stream of the specified asynchronous request.
		/// </summary>
		/// <param name="result">The result of the asynchronous operation.</param>
		protected void EndStreamRead(IAsyncResult result)
		{
			// Get the state of the asynchronous operation
			AsyncWebResult asyncState = (AsyncWebResult)result.AsyncState;

			try
			{
				// If the asynchrounous operation is not completed
				if (!result.IsCompleted)
				{
					// Wait on the operation handle until the timeout expires
					if (!result.AsyncWaitHandle.WaitOne(this.Timeout))
					{
						// If no signal was received in the timeout period, throw a timeout exception.
						throw new TimeoutException(string.Format("The read request did not complete within the timeout {0}.", this.Timeout));
					}
				}

				// Complete the asynchronous request and get the bytes read.
				int count = asyncState.Stream.EndRead(result);

				// Append the bytes read to the string buffer.
				asyncState.ReceiveData.Append(asyncState.Buffer, count);

				if (count > 0)
				{
					// If bytes were read, begin a new read request.
					this.BeginStreamRead(asyncState);
				}
				else
				{
					// Close the response stream.
					asyncState.Response.Close();
					// Otherwise, signal that the asynchronous operation has completed.
					asyncState.Complete();
					// Call the callback function.
					if (asyncState.Callback != null) asyncState.Callback(asyncState);
					// Dispose the asynchronous state.
					asyncState.Dispose();
				}
			}
			catch (Exception exception)
			{
				// Close the response stream.
				asyncState.Response.Close();
				// If an exception occured, set the exception.
				asyncState.Exception = exception;
				// Complete the asynchronous operation.
				asyncState.Complete();
				// Call the callback method.
				if (asyncState.Callback != null) asyncState.Callback(asyncState);
				// Dispose the asynchronous state.
				asyncState.Dispose();
			}
		}
	}
}
