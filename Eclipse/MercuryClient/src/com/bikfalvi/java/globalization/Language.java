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

import java.util.Locale;

/**
 * A class representing a language.
 * @author Alex Bikfalvi
 *
 */
public class Language {
	private String type;
	private String name;
	
	/**
	 * Creates a new language instance.
	 * @param type The language type.
	 * @param name The language name.
	 */
	public Language(String type, String name) {
		this.type = type.toLowerCase(Locale.US);
		this.name = name;
	}
	
	/**
	 * Gets the language type.
	 * @return The type.
	 */
	public String getType() {
		return type;
	}

	/**
	 * Sets the language type.
	 * @param type The type.
	 */
	public void setType(String type) {
		this.type = type;
	}

	/**
	 * Gets the language name.
	 * @return The name.
	 */
	public String getName() {
		return name;
	}

	/**
	 * Sets the language name.
	 * @param name The name.
	 */
	public void setName(String name) {
		this.name = name;
	}
	
	@Override
	public boolean equals(Object object) {
		if (null == object) return false;
		if (object instanceof Language) {
			Language language = (Language)object;
			return this.type.equals(language.type);
		}
		else if (object instanceof String) {
			String language = (String) object;
			return this.type.equals(language);
		}
		else return false;
	}
	
	@Override
	public int hashCode() {
		return this.type.hashCode();
	}

	@Override
	public String toString() {
		return this.name;
	}
}
