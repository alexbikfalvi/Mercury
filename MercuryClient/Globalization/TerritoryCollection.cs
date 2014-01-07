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
	/// A class that represents a territory collection.
	/// </summary>
	[Serializable]
	public class TerritoryCollection : IEnumerable<Territory>
	{
		private readonly Dictionary<string, Territory> territories = new Dictionary<string, Territory>();

		/// <summary>
		/// Creates an empty territory collection instance for the specified culture.
		/// </summary>
		/// <param name="culture">The culture.</param>
		public TerritoryCollection(CultureId culture)
		{
			this.Culture = culture;
		}

		/// <summary>
		/// Creates a territory collection instance for the specified culture.
		/// </summary>
		/// <param name="culture">The culture.</param>
		/// <param name="territories">An enumeration of territories.</param>
		public TerritoryCollection(CultureId culture, IEnumerable<Territory> territories)
		{
			this.Culture = culture;

			foreach (Territory territory in territories)
			{
				this.territories.Add(territory.Type, territory);
			}
		}

		// Public properties.

		/// <summary>
		/// Gets the collection culture.
		/// </summary>
		public CultureId Culture { get; private set; }
		/// <summary>
		/// Gets the number of territories in the collection.
		/// </summary>
		public int Count
		{
			get { return this.territories.Count; }
		}

		// Public methods.

		/// <summary>
		/// Gets the enumerator for the territory collection.
		/// </summary>
		/// <returns>The collection enumerator.</returns>
		public IEnumerator<Territory> GetEnumerator()
		{
			return this.territories.Values.GetEnumerator();
		}

		/// <summary>
		/// Gets the enumerator for the territory collection.
		/// </summary>
		/// <returns>The collection enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>
		/// Adds a new territory to the collection.
		/// </summary>
		/// <param name="culture">The territory culture.</param>
		/// <param name="name">The territory name.</param>
		public void Add(string type, string name)
		{
			this.territories.Add(type, new Territory(type, name));
		}

		/// <summary>
		/// Determines whether the collection contains a territory for the specified culture.
		/// </summary>
		/// <param name="culture">The culture.</param>
		public bool Contains(string culture)
		{
			return this.territories.ContainsKey(culture);
		}

		/// <summary>
		/// Gets the territory for the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="territory">The territory.</param>
		/// <returns><b>True</b> if the collection contains a territory for the specified type, <b>false</b> otherwise.</returns>
		public bool TryGetTerritory(string type, out Territory territory)
		{
			return this.territories.TryGetValue(type, out territory);
		}
	}
}
