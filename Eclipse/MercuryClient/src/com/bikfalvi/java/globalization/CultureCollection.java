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
 * A class representing a culture collection.
 * @author Alex Bikfalvi
 *
 */
public class CultureCollection implements Iterable<Culture> {
	private final Hashtable<CultureId, Culture> cultures;

	/**
	 * Creates an empty culture collection instance.
	 */
	public CultureCollection() {
		this.cultures = new Hashtable<CultureId, Culture>();
	}

	/**
	 * Creates a culture collection instance.
	 * @param cultures An iteration of cultures.
	 */
	public CultureCollection(Iterable<Culture> cultures) {
		// Validate the arguments.
		if (null == cultures) throw new IllegalArgumentException("cultures");

		this.cultures = new Hashtable<CultureId, Culture>();
		for (Culture culture : cultures) {
			this.cultures.put(culture.getId(), culture);
		}
	}
	
	/**
	 * Returns the number of cultures in the collection.
	 * @return
	 */
	public int size() {
		return this.cultures.size();
	}
	
	/**
	 * Gets the culture with the specified type.
	 * @param type The culture type.
	 * @return The culture.
	 */
	public Culture getCulture(String type) {
		return this.cultures.get(new CultureId(type));
	}
	
	/**
	 * Gets the culture with the specified culture identifier.
	 * @param id The culture identifier.
	 * @return The culture.
	 */
	public Culture getCulture(CultureId id) {
		return this.cultures.get(id);
	}
	
	/**
	 * Gets the culture with the specified language.
	 * @param language The language.
	 * @return The culture.
	 */
	public Culture getCulture(Language language) {
		return this.cultures.get(new CultureId(language.getType()));
	}

	/**
	 * Adds a new culture to the collection.
	 * @param culture The culture.
	 */
	public void add(Culture culture) {
		// Validate the argument.
		if (null == culture) throw new IllegalArgumentException("culture");
		// Add the locale.
		this.cultures.put(culture.getId(), culture);
	}

	/**
	 * Determines whether the collection contains a locale for the specified culture.
	 * @param id The culture identifier.
	 * @return True if the collection contains the culture, false otherwise.
	 */
	public boolean contains(CultureId id) {
		return this.cultures.containsKey(id);
	}

	@Override
	public Iterator<Culture> iterator() {
		return this.cultures.values().iterator();
	}
}

