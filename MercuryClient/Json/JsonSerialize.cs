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
	/// A class used for serialization of JSON objects.
	/// </summary>
	public static class JsonSerialize
	{
		/// <summary>
		/// Serializes the specified object to JSON.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The JSON representation.</returns>
		public static string Serialize(object value)
		{
			if (value is JsonObject) return value.ToString();
			else if (value is JsonProperty) return value.ToString();
			else if (value is JsonArray) return value.ToString();
			else if (value is string) return string.Format("\"{0}\"", value as string);
			else throw new NotSupportedException(string.Format("The object of type {0} is not supported.", value.GetType()));
		}
	}
}
