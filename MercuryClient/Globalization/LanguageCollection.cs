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
	/// A class that represents a language collection.
	/// </summary>
	[Serializable]
	public class LanguageCollection : IEnumerable<Language>
	{
		private readonly Dictionary<string, Language> languages = new Dictionary<string, Language>();

		/// <summary>
		/// Creates an empty language collection instance for the specified culture.
		/// </summary>
		/// <param name="culture">The culture.</param>
		public LanguageCollection(CultureId culture)
		{
			this.Culture = culture;
		}

		/// <summary>
		/// Creates a language collection instance for the specified culture.
		/// </summary>
		/// <param name="culture">The culture.</param>
		/// <param name="languages">An enumeration of languages.</param>
		public LanguageCollection(CultureId culture, IEnumerable<Language> languages)
		{
			this.Culture = culture;

			foreach (Language language in languages)
			{
				this.languages.Add(language.Type, language);
			}
		}

		// Public properties.

		/// <summary>
		/// Gets the collection culture.
		/// </summary>
		public CultureId Culture { get; private set; }
		/// <summary>
		/// Gets the number of languages in the collection.
		/// </summary>
		public int Count
		{
			get { return this.languages.Count; }
		}
		/// <summary>
		/// Gets the language for the specified language.
		/// </summary>
		/// <param name="language">The language.</param>
		/// <returns>The language.</returns>
		public Language this[string language]
		{
			get { return this.languages[language]; }
		}

		// Public methods.

		/// <summary>
		/// Gets the enumerator for the language collection.
		/// </summary>
		/// <returns>The collection enumerator.</returns>
		public IEnumerator<Language> GetEnumerator()
		{
			return this.languages.Values.GetEnumerator();
		}

		/// <summary>
		/// Gets the enumerator for the language collection.
		/// </summary>
		/// <returns>The collection enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>
		/// Adds a new language to the collection.
		/// </summary>
		/// <param name="type">The language type.</param>
		/// <param name="name">The language name.</param>
		public void Add(string type, string name)
		{
			this.languages.Add(type, new Language(type, name));
		}

		/// <summary>
		/// Determines whether the collection contains a language for the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		public bool Contains(string type)
		{
			return this.languages.ContainsKey(type);
		}

		/// <summary>
		/// Gets the language for the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="language">The language.</param>
		/// <returns><b>True</b> if the collection contains a language for the specified type, <b>false</b> otherwise.</returns>
		public bool TryGetLanguage(string type, out Language language)
		{
			return this.languages.TryGetValue(type, out language);
		}
	}
}
