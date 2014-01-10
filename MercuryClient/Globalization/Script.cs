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

namespace Mercury.Globalization
{
	/// <summary>
	/// A class representing a script.
	/// </summary>
	[Serializable]
	public class Script
	{
		/// <summary>
		/// Creates a new script instance.
		/// </summary>
		/// <param name="type">The script type.</param>
		/// <param name="name">The script name.</param>
		public Script(string type, string name)
		{
			this.Type = type.ToLowerInvariant();
			this.Name = name;
		}

		// Public properties.

		/// <summary>
		/// Gets the script type.
		/// </summary>
		public string Type { get; private set; }
		/// <summary>
		/// Gets the script name.
		/// </summary>
		public string Name { get; private set; }
		
		// Public methods.

		/// <summary>
		/// Gets the script name.
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
		/// <returns><b>True</b> if the objects are the equal, <b>false</b> otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (null == obj) return false;
			Script script = obj as Script;
			if (null == script) return false;
			return this.Type == script.Type;
		}

		/// <summary>
		/// Returns the hash code of the current object.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			return this.Type.GetHashCode();
		}

		/// <summary>
		/// Compares two script objects for equality.
		/// </summary>
		/// <param name="left">The left script.</param>
		/// <param name="right">The right script.</param>
		/// <returns><b>True</b> if the scripts are the equal, <b>false</b> otherwise.</returns>
		public static bool operator ==(Script left, Script right)
		{
			if (object.ReferenceEquals(left, right)) return true;
			if (((object)left == null) || ((object)right == null)) return false;
			return left.Type == right.Type;
		}

		/// <summary>
		/// Compares two script objects for inequality.
		/// </summary>
		/// <param name="left">The left script.</param>
		/// <param name="right">The right script.</param>
		/// <returns><b>True</b> if the scripts are the different, <b>false</b> otherwise.</returns>
		public static bool operator !=(Script left, Script right)
		{
			return !(left == right);
		}
	}
}
