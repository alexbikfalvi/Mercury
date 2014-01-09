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

namespace DotNetApi.Globalization
{
	/// <summary>
	/// A class representing a language.
	/// </summary>
	[Serializable]
	public class Language
	{
		/// <summary>
		/// Creates a new language instance.
		/// </summary>
		/// <param name="type">The language type.</param>
		/// <param name="name">The language name.</param>
		public Language(string type, string name)
		{
			this.Type = type;
			this.Name = name;
		}

		/// <summary>
		/// Gets the language type.
		/// </summary>
		public string Type { get; private set; }
		/// <summary>
		/// Gets the language name.
		/// </summary>
		public string Name { get; private set; }

		// Public methods.

		/// <summary>
		/// Gets the language name.
		/// </summary>
		/// <returns>The name.</returns>
		public override string ToString()
		{
			return this.Name;
		}

		/// <summary>
		/// Compares with an object for equality.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns><b>True</b> if the languages are the same, <b>false</b> otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (null == obj) return false;
			if (!(obj is Language)) return false;
			return this == (obj as Language);
		}
	}
}
