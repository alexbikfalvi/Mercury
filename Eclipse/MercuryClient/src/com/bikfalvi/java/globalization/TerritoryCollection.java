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
 * A class that represents a territory collection.
 * @author Alex Bikfalvi
 *
 */
public class TerritoryCollection implements Iterable<Territory> {
	private final Hashtable<String, Territory> territories = new Hashtable<String, Territory>();
	private final CultureId culture;

	/**
	 * Creates an empty territory collection instance for the specified culture.
	 * @param culture The culture.
	 */
	public TerritoryCollection(CultureId culture) {
		this.culture = culture;
	}

	/**
	 * Creates a territory collection instance for the specified culture.
	 * @param culture The culture.
	 * @param territories An enumeration of territories.
	 */
	public TerritoryCollection(CultureId culture, Iterable<Territory> territories) {
		this.culture = culture;

		for (Territory territory : territories) {
			this.territories.put(territory.getType(), territory);
		}
	}

	/**
	 * Gets the collection culture. 
	 */
	public CultureId getCulture() {
		return culture;
	}
	
	/**
	 * Returns the number of territories in the collection.
	 * @return
	 */
	public int size() {
		return this.territories.size();
	}
	
	/**
	 * Gets the territory with the specified type.
	 * @param type The territory type.
	 * @return The territory.
	 */
	public Territory getTerritory(String type) {
		return this.territories.get(type);
	}
	
	/**
	 * Adds a new territory to the collection.
	 * @param type The territory type.
	 * @param name The territory name.
	 */
	public void add(String type, String name) {
		this.territories.put(type, new Territory(type, name));
	}
	
	/**
	 * Checks whether the collection contains a territory with the specified type.
	 * @param type The territory type.
	 * @return True if the collection contains the territory, false otherwise.
	 */
	public boolean contains(String type) {
		return this.territories.containsKey(type);
	}

	@Override
	public Iterator<Territory> iterator() {
		return this.territories.values().iterator();
	}	
}

