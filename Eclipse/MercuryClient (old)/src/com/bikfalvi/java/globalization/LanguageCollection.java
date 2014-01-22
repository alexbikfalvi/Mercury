/** 
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

package com.bikfalvi.java.globalization;

import java.util.Hashtable;
import java.util.Iterator;

/**
 * A class that represents a language collection.
 * @author Alex Bikfalvi
 *
 */
public class LanguageCollection implements Iterable<Language> {
	private final Hashtable<String, Language> languages = new Hashtable<String, Language>();
	private final CultureId culture;

	/**
	 * Creates an empty language collection instance for the specified culture.
	 * @param culture The culture.
	 */
	public LanguageCollection(CultureId culture) {
		this.culture = culture;
	}

	/**
	 * Creates a language collection instance for the specified culture.
	 * @param culture The culture.
	 * @param languages An enumeration of languages.
	 */
	public LanguageCollection(CultureId culture, Iterable<Language> languages) {
		this.culture = culture;

		for (Language language : languages) {
			this.languages.put(language.getType(), language);
		}
	}

	/**
	 * Gets the collection culture. 
	 */
	public CultureId getCulture() {
		return culture;
	}
	
	/**
	 * Returns the number of languages in the collection.
	 * @return
	 */
	public int size() {
		return this.languages.size();
	}
	
	/**
	 * Gets the language with the specified type.
	 * @param type The language type.
	 * @return The language.
	 */
	public Language getLanguage(String type) {
		return this.languages.get(type);
	}
	
	/**
	 * Adds a new language to the collection.
	 * @param type The language type.
	 * @param name The language name.
	 */
	public void add(String type, String name) {
		this.languages.put(type, new Language(type, name));
	}
	
	/**
	 * Checks whether the collection contains a language with the specified type.
	 * @param type The language type.
	 * @return True if the collection contains the language, false otherwise.
	 */
	public boolean contains(String type) {
		return this.languages.containsKey(type);
	}

	@Override
	public Iterator<Language> iterator() {
		return this.languages.values().iterator();
	}	
}

