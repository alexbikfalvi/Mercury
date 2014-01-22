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

import com.bikfalvi.java.StringExtensions;

/**
 * A class representing the culture identifier.
 * @author Alex Bikfalvi
 *
 */
public class CultureId {
	private String language;
	private String script;
	private String territory;
	
	/**
	 * Creates a new culture identifier for the specified language.
	 * @param language The language.
	 */
	public CultureId(String language) {
		// Validate the arguments.
		if (null == language) throw new IllegalArgumentException("language");

		this.language = language;
		this.script = null;
		this.territory = null;
	}

	/**
	 * Creates a new culture identifier for the specified language, script and territory.
	 * @param language The language.
	 * @param script The script.
	 * @param territory The territory.
	 */
	public CultureId(String language, String script, String territory) {
		// Validate the arguments.
		if (null == language) throw new IllegalArgumentException("language");

		this.language = language;
		this.script = script;
		this.territory = territory;
	}

	/**
	 * Gets the culture language.
	 * @return The language.
	 */
	public String getLanguage() {
		return language;
	}

	/**
	 * Sets the language.
	 * @param language The language.
	 */
	public void setLanguage(String language) {
		this.language = language;
	}

	/**
	 * Gets the script.
	 * @return The script.
	 */
	public String getScript() {
		return script;
	}

	/**
	 * Sets the script.
	 * @param script The script.
	 */
	public void setScript(String script) {
		this.script = script;
	}

	/**
	 * Gets the territory.
	 * @return The territory.
	 */
	public String getTerritory() {
		return territory;
	}

	/**
	 * Sets the territory.
	 * @param territory The territory.
	 */
	public void setTerritory(String territory) {
		this.territory = territory;
	}

	@Override
	public boolean equals(Object object) {
		if (null == object) return false;
		if (!(object instanceof CultureId)) return false;
		CultureId id = (CultureId)object;
		return StringExtensions.equalsIgnoreCase(this.language, id.language) &&
				StringExtensions.equalsIgnoreCase(this.script, id.script) &&
				StringExtensions.equalsIgnoreCase(this.territory, id.territory);
	}
	
	@Override
	public int hashCode() {
		return this.language.hashCode() ^ 
				(null != this.script ? this.script.hashCode() : 0) ^ 
				(null != this.territory ? this.territory.hashCode() : 0);
	}
}
