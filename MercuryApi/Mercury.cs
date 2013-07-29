/* 
 * Copyright (C) 2013 Alex Bikfalvi
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
using Microsoft.Win32;

namespace MercuryApi
{
	/// <summary>
	/// A class representing the main class of Mercury API.
	/// </summary>
    public class Mercury : IDisposable
    {
		private MercuryConfig config;

		/// <summary>
		/// Creates a new Mercury instance.
		/// </summary>
		/// <param name="rootKey">The root key of the Mercury registry configuration.</param>
		/// <param name="rootPath">The root path of the Mercury registry configuration.</param>
		public Mercury(RegistryKey rootKey, string rootPath)
		{
			// Create the configuration.
			this.config = new MercuryConfig(rootKey, rootPath);
		}

		// Public properties.

		/// <summary>
		/// Gets the Mercury configuration.
		/// </summary>
		public MercuryConfig Config { get { return this.config;  } }

		// Public methods.

		/// <summary>
		/// Disposes the current object.
		/// </summary>
		public void Dispose()
		{
			// Dispose the configuration.
			this.config.Dispose();
		}
    }
}
