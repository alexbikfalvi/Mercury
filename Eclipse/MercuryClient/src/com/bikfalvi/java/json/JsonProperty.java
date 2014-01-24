/* 
 * Copyright (C) 2014 Alex Bikfalvi
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

package com.bikfalvi.java.json;

/**
 * A class representing a JSON property.
 * @author Alex Bikfalvi
 *
 */
public class JsonProperty
{
	private final String name;
	private final Object value;

	/**
	 * Creates a new JSON property.
	 * @param name The property name.
	 * @param value The property value.
	 */
	public JsonProperty(String name, Object value)
	{
		this.name = name;
		this.value = value;
	}

	@Override
	public String toString()
	{
		return String.format("\"%s\" : %s", this.name, JsonSerialize.serialize(this.value));
	}
}
