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
using System.Collections.Generic;
using System.Text;

namespace Mercury.Json
{
	/// <summary>
	/// A class representing a JSON object.
	/// </summary>
	public class JsonObject
	{
		private readonly List<object> contents = new List<object>();

		/// <summary>
		/// Creates an empty JSON object.
		/// </summary>
		public JsonObject()
		{
		}

		/// <summary>
		/// Creates a JSON object with the specified contents.
		/// </summary>
		/// <param name="contens">The JSON object contents.</param>
		public JsonObject(params object[] contens)
		{
			this.contents.AddRange(contens);
		}

		// Public methods.

		/// <summary>
		/// Converts the JSON object to a string.
		/// </summary>
		/// <returns>The JSON string.</returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("{ ");
			for (int index = 0; index < this.contents.Count; index++)
			{
				if (index > 0) builder.Append(" , ");
				builder.Append(JsonSerialize.Serialize(this.contents[index]));
			}
			builder.Append(" }");

			return builder.ToString();
		}
	}
}
