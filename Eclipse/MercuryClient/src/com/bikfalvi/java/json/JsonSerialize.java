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
 * A class used for serialization of JSON objects.
 * @author Alex Bifkalvi
 *
 */
public final class JsonSerialize
{
	/**
	 * Serializes the specified object to JSON.
	 * @param value The value.
	 * @return The JSON representation.
	 */
	public static String serialize(Object value)
	{
		if (value instanceof JsonObject) return value.toString();
		else if (value instanceof JsonProperty) return value.toString();
		else if (value instanceof JsonArray) return value.toString();
		else if (value instanceof String) return String.format("\"%s\"", value);
		else throw new UnsupportedOperationException(String.format("The object of type %s is not supported.", value.getClass()));
	}
}
