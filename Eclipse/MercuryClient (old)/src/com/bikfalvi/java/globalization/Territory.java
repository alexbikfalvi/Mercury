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
 * A class representing a territory.
 * @author Alex Bikfalvi
 *
 */
public class Territory {
	private String type;
	private String name;
	
	/**
	 * Creates a new territory instance.
	 * @param type The territory type.
	 * @param name The territory name.
	 */
	public Territory(String type, String name) {
		this.type = type.toLowerCase(Locale.US);
		this.name = name;
	}
	
	/**
	 * Gets the territory type.
	 * @return The type.
	 */
	public String getType() {
		return type;
	}

	/**
	 * Sets the territory type.
	 * @param type The type.
	 */
	public void setType(String type) {
		this.type = type;
	}

	/**
	 * Gets the territory name.
	 * @return The name.
	 */
	public String getName() {
		return name;
	}

	/**
	 * Sets the territory name.
	 * @param name The name.
	 */
	public void setName(String name) {
		this.name = name;
	}
	
	@Override
	public boolean equals(Object object) {
		if (null == object) return false;
		if (object instanceof Territory) {
			Territory territory = (Territory)object;
			return this.type.equals(territory.type);
		}
		else if (object instanceof String) {
			String territory = (String) object;
			return this.type.equals(territory);
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
