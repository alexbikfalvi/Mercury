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
using System.Collections.Generic;

namespace Mercury.Globalization
{
	/// <summary>
	/// A class representing a locale.
	/// </summary>
	[Serializable]
	public class Locale
	{
		private readonly LanguageCollection languages;
		private readonly ScriptCollection scripts;
		private readonly TerritoryCollection territories;

		/// <summary>
		/// Creates a locale instance for the specified culture.
		/// </summary>
		/// <param name="culture">The culture.</param>
		public Locale(CultureId culture)
		{
			this.languages = new LanguageCollection(culture);
			this.scripts = new ScriptCollection(culture);
			this.territories = new TerritoryCollection(culture);
			this.Culture = culture;
		}

		/// <summary>
		/// Creates a locale instance for the specified culture.
		/// </summary>
		/// <param name="culture">The culture.</param>
		/// <param name="languages">An enumeration of languages.</param>
		/// <param name="scripts">An enumeration of scripts.</param>
		/// <param name="territories">An enumeration of territories.</param>
		public Locale(CultureId culture, IEnumerable<Language> languages, IEnumerable<Script> scripts, IEnumerable<Territory> territories)
		{
			this.languages = new LanguageCollection(culture, languages);
			this.scripts = new ScriptCollection(culture, scripts);
			this.territories = new TerritoryCollection(culture, territories);
			this.Culture = culture;
		}

		// Public properties.

		/// <summary>
		/// Gets the locale culture.
		/// </summary>
		public CultureId Culture { get; private set; }
		/// <summary>
		/// Gets the locale languages.
		/// </summary>
		public LanguageCollection Languages { get { return this.languages; } }
		/// <summary>
		/// Gets the locale scripts.
		/// </summary>
		public ScriptCollection Scripts { get { return this.scripts; } }
		/// <summary>
		/// Gets the locale territories.
		/// </summary>
		public TerritoryCollection Territories { get { return this.territories; } }
	}
}
