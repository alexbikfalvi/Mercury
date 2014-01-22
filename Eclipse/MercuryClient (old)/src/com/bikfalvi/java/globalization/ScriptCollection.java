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
 * A class that represents a script collection.
 * @author Alex Bikfalvi
 *
 */
public class ScriptCollection implements Iterable<Script> {
	private final Hashtable<String, Script> scripts = new Hashtable<String, Script>();
	private final CultureId culture;

	/**
	 * Creates an empty script collection instance for the specified culture.
	 * @param culture The culture.
	 */
	public ScriptCollection(CultureId culture) {
		this.culture = culture;
	}

	/**
	 * Creates a script collection instance for the specified culture.
	 * @param culture The culture.
	 * @param scripts An enumeration of scripts.
	 */
	public ScriptCollection(CultureId culture, Iterable<Script> scripts) {
		this.culture = culture;

		for (Script script : scripts) {
			this.scripts.put(script.getType(), script);
		}
	}

	/**
	 * Gets the collection culture. 
	 */
	public CultureId getCulture() {
		return culture;
	}
	
	/**
	 * Returns the number of scripts in the collection.
	 * @return
	 */
	public int size() {
		return this.scripts.size();
	}
	
	/**
	 * Gets the script with the specified type.
	 * @param type The script type.
	 * @return The script.
	 */
	public Script getScript(String type) {
		return this.scripts.get(type);
	}
	
	/**
	 * Adds a new script to the collection.
	 * @param type The script type.
	 * @param name The script name.
	 */
	public void add(String type, String name) {
		this.scripts.put(type, new Script(type, name));
	}
	
	/**
	 * Checks whether the collection contains a script with the specified type.
	 * @param type The script type.
	 * @return True if the collection contains the script, false otherwise.
	 */
	public boolean contains(String type) {
		return this.scripts.containsKey(type);
	}

	@Override
	public Iterator<Script> iterator() {
		return this.scripts.values().iterator();
	}	
}

