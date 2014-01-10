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
using System.Text;

namespace Mercury.Web
{
	/// <summary>
	/// A class that represents an asynchronous web buffer.
	/// </summary>
	public class AsyncWebBuffer
	{
		private byte[] data = null;

		// Public properties.

		/// <summary>
		/// Gets the data stored in the buffer.
		/// </summary>
		public byte[] Data { get { return this.data; } }

		// Public methods.

		/// <summary>
		/// Appends byte data to the current buffer.
		/// </summary>
		/// <param name="data">A byte array containing the data to append.</param>
		public void Append(byte[] data)
		{
			// Get the size of the old buffer.
			int sizeOld = (null != this.data) ? this.data.Length : 0;
			// Compute the size of the new buffer.
			int sizeNew = sizeOld + data.Length;
			// Allocate a new buffer.
			byte[] buffer = new byte[sizeNew];
			// Copy the old buffer into the new buffer.
			if (null != this.data)
			{
				Buffer.BlockCopy(this.data, 0, buffer, 0, sizeOld);
			}
			// Append the new data into the new buffer.
			Buffer.BlockCopy(data, 0, buffer, sizeOld, data.Length);
			// Assign the new buffer.
			this.data = buffer;
		}

		/// <summary>
		/// Appends byte data to the current buffer.
		/// </summary>
		/// <param name="data">A byte array containing the data to append.</param>
		/// <param name="count">The number of bytes to append.</param>
		public void Append(byte[] data, int count)
		{
			// Get the size of the old buffer.
			int sizeOld = (null != this.data) ? this.data.Length : 0;
			// Compute the size of the new buffer.
			int sizeNew = sizeOld + count;
			// Allocate a new buffer.
			byte[] buffer = new byte[sizeNew];
			// Copy the old buffer into the new buffer.
			if (null != this.data)
			{
				Buffer.BlockCopy(this.data, 0, buffer, 0, sizeOld);
			}
			// Append the new data into the new buffer.
			Buffer.BlockCopy(data, 0, buffer, sizeOld, count);
			// Assign the new buffer.
			this.data = buffer;
		}

		/// <summary>
		/// Appends the string data to the current buffer, using the specified encoding.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="encoding">The encoding.</param>
		public void Append(string data, Encoding encoding)
		{
			// Get the number of bytes for the specified data string.
			int count = encoding.GetByteCount(data);
			// Get the byte data for the specified data string.
			byte[] bytes = encoding.GetBytes(data);
			// Get the size of the old buffer.
			int sizeOld = (null != this.data) ? this.data.Length : 0;
			// Compute the size of the new buffer.
			int sizeNew = sizeOld + count;
			// Allocate a new buffer.
			byte[] buffer = new byte[sizeNew];
			// Copy the old buffer into the new buffer.
			if (null != this.data)
			{
				Buffer.BlockCopy(this.data, 0, buffer, 0, sizeOld);
			}
			// Append the new data into the new buffer.
			Buffer.BlockCopy(bytes, 0, buffer, sizeOld, count);
			// Assign the new buffer.
			this.data = buffer;
		}

		/// <summary>
		/// Returns the data as a string using UTF-8 encoding.
		/// </summary>
		/// <returns>The data as a string.</returns>
		public override string ToString()
		{
			return this.data != null ? Encoding.UTF8.GetString(this.data) : string.Empty;
		}

		/// <summary>
		/// Returns the data as a string using the specified encoding.
		/// </summary>
		/// <param name="encoding">The encoding.</param>
		/// <returns>The data as a string.</returns>
		public string ToString(Encoding encoding)
		{
			return this.data != null ? encoding.GetString(this.data) : string.Empty;
		}
	};
}
