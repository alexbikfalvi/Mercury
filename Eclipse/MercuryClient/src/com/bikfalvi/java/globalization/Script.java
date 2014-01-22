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
 * A class representing a script.
 * @author Alex Bikfalvi
 *
 */
public class Script
{
	private String type;
	private String name;
	
	/**
	 * Creates a new script instance.
	 * @param type The script type.
	 * @param name The script name.
	 */
	public Script(String type, String name) {
		this.type = type.toLowerCase(Locale.US);
		this.name = name;
	}
	
	/**
	 * Gets the script type.
	 * @return The type.
	 */
	public String getType() {
		return type;
	}

	/**
	 * Sets the script type.
	 * @param type The type.
	 */
	public void setType(String type) {
		this.type = type;
	}

	/**
	 * Gets the script name.
	 * @return The name.
	 */
	public String getName() {
		return name;
	}

	/**
	 * Sets the script name.
	 * @param name The name.
	 */
	public void setName(String name) {
		this.name = name;
	}
	
	@Override
	public boolean equals(Object object) {
		if (null == object) return false;
		if (object instanceof Script) {
			Script script = (Script)object;
			return this.type.equals(script.type);
		}
		else if (object instanceof String) {
			String script = (String) object;
			return this.type.equals(script);
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
