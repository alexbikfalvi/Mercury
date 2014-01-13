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
using System.IO;
using System.Runtime;
using System.Runtime.Serialization.Formatters.Binary;
using Mercury.Globalization;

namespace Mercury.Globalization
{
	/// <summary>
	/// A class representing the locale resources.
	/// </summary>
	internal static class Resources
	{
		private static LocaleCollection locales;

		/// <summary>
		/// Initializes the locales resources.
		/// </summary>
		static Resources()
		{
			// Create the binary formatter.
			BinaryFormatter formatter = new BinaryFormatter();

			using (MemoryStream stream = new MemoryStream(Mercury.Locales.Collection))
			{
				Resources.locales = formatter.Deserialize(stream) as LocaleCollection;
			}
		}

		// Public properties.

		/// <summary>
		/// Gets the locales collection.
		/// </summary>
		internal static LocaleCollection Locales { get { return Resources.locales; } }
	}
}
