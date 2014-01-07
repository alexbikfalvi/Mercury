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
using System.Text;

namespace DotNetApi.Globalization
{
	/// <summary>
	/// A structure representing the culture identifier.
	/// </summary>
	[Serializable]
	public struct CultureId
	{
		/// <summary>
		/// Creates a new culture identifier for the specified language.
		/// </summary>
		/// <param name="language">The language.</param>
		public CultureId(string language)
			: this()
		{
			// Validate the arguments.
			if (null == language) throw new ArgumentNullException("language");

			this.Language = language;
			this.Script = null;
			this.Territory = null;
		}

		/// <summary>
		/// Creates a new culture identifier for the specified language, script and territory.
		/// </summary>
		/// <param name="language">The language.</param>
		/// <param name="script">The script.</param>
		/// <param name="territory">The territory.</param>
		public CultureId(string language, string script, string territory)
			: this()
		{
			// Validate the arguments.
			if (null == language) throw new ArgumentNullException("language");

			this.Language = language;
			this.Script = script;
			this.Territory = territory;
		}

		// Public properties.

		/// <summary>
		/// The culture language.
		/// </summary>
		public string Language { get; private set; }
		/// <summary>
		/// The culture script.
		/// </summary>
		public string Script { get; private set; }
		/// <summary>
		/// The culture territory.
		/// </summary>
		public string Territory { get; private set; }

		// Public methods.

		/// <summary>
		/// Compares two culture identifier objects for equality.
		/// </summary>
		/// <param name="left">The left culture identifier.</param>
		/// <param name="right">The right culture identifier.</param>
		/// <returns><b>True</b> if the two culture identifiers are equal, <b>false</b> otherwise.</returns>
		public static bool operator ==(CultureId left, CultureId right)
		{
			return (left.Language == right.Language) && (left.Script == right.Script) && (left.Territory == right.Territory);
		}

		/// <summary>
		/// Compares two culture identifier objects for inequality.
		/// </summary>
		/// <param name="left">The left culture identifier.</param>
		/// <param name="right">The right culture identifier.</param>
		/// <returns><b>True</b> if the two culture identifiers are different, <b>false</b> otherwise.</returns>
		public static bool operator !=(CultureId left, CultureId right)
		{
			return (left.Language != right.Language) || (left.Script != right.Script) || (left.Territory != right.Territory);
		}

		/// <summary>
		/// Compares two culture identifier objects.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns><b>True</b> if the objects are equal, otherwise false.</returns>
		public override bool Equals(object obj)
		{
			if (null == obj) return false;
			if (!(obj is CultureId)) return false;
			CultureId id = (CultureId)obj;
			return this == id;
		}

		/// <summary>
		/// Gets the hash code for the culture identifier.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			return this.Language.GetHashCode() ^
				(this.Script != null ? this.Script.GetHashCode() : 0) ^
				(this.Territory != null ? this.Territory.GetHashCode() : 0);
		}

		/// <summary>
		/// Converts the culture identifier to a string.
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			// Create a string builder.
			StringBuilder builder = new StringBuilder(this.Language);
			// Append the script.
			if (null != this.Script)
			{
				builder.Append("_");
				builder.Append(this.Script);
			}
			// Append the territory.
			if (null != this.Territory)
			{
				builder.Append("_");
				builder.Append(this.Territory);
			}
			// Return the builder string.
			return builder.ToString();
		}
	}
}
