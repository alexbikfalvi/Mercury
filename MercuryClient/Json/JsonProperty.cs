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

namespace Mercury.Json
{
	/// <summary>
	/// A class representing a JSON property.
	/// </summary>
	public class JsonProperty
	{
		private readonly string name;
		private readonly object value;

		/// <summary>
		/// Creates a new JSON property.
		/// </summary>
		/// <param name="name">The property name.</param>
		/// <param name="value">The property value.</param>
		public JsonProperty(string name, object value)
		{
			this.name = name;
			this.value = value;
		}

		// Public methods.

		/// <summary>
		/// Converts the JSON property to a string.
		/// </summary>
		/// <returns>The JSON string.</returns>
		public override string ToString()
		{
			return string.Format("\"{0}\" : {1}", this.name, JsonSerialize.Serialize(this.value));
		}
	}
}
