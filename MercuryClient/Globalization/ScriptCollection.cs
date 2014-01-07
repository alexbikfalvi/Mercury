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
using System.Collections;
using System.Collections.Generic;

namespace DotNetApi.Globalization
{
	/// <summary>
	/// A class that represents a script collection.
	/// </summary>
	[Serializable]
	public class ScriptCollection : IEnumerable<Script>
	{
		private readonly Dictionary<string, Script> scripts = new Dictionary<string, Script>();

		/// <summary>
		/// Creates an empty script collection instance for the specified culture.
		/// </summary>
		/// <param name="culture">The culture.</param>
		public ScriptCollection(CultureId culture)
		{
			this.Culture = culture;
		}

		/// <summary>
		/// Creates a script collection instance for the specified culture.
		/// </summary>
		/// <param name="culture">The culture.</param>
		/// <param name="scripts">An enumerable of scripts.</param>
		public ScriptCollection(CultureId culture, IEnumerable<Script> scripts)
		{
			this.Culture = culture;

			foreach (Script script in scripts)
			{
				this.scripts.Add(script.Type, script);
			}
		}

		// Public properties.

		/// <summary>
		/// Gets the collection culture.
		/// </summary>
		public CultureId Culture { get; private set; }
		/// <summary>
		/// Gets the number of scripts in the collection.
		/// </summary>
		public int Count
		{
			get { return this.scripts.Count; }
		}

		// Public methods.

		/// <summary>
		/// Gets the enumerator for the script collection.
		/// </summary>
		/// <returns>The collection enumerator.</returns>
		public IEnumerator<Script> GetEnumerator()
		{
			return this.scripts.Values.GetEnumerator();
		}

		/// <summary>
		/// Gets the enumerator for the script collection.
		/// </summary>
		/// <returns>The collection enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>
		/// Adds a new script to the collection.
		/// </summary>
		/// <param name="type">The script type.</param>
		/// <param name="name">The script name.</param>
		public void Add(string type, string name)
		{
			this.scripts.Add(type, new Script(type, name));
		}

		/// <summary>
		/// Determines whether the collection contains a script for the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		public bool Contains(string type)
		{
			return this.scripts.ContainsKey(type);
		}

		/// <summary>
		/// Gets the script for the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="script">The script.</param>
		/// <returns><b>True</b> if the collection contains a script for the specified type, <b>false</b> otherwise.</returns>
		public bool TryGetScript(string type, out Script script)
		{
			return this.scripts.TryGetValue(type, out script);
		}
	}
}
