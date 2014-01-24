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

import java.util.ArrayList;

/**
 * A class representing a JSON array.
 * @author Alex Bikfalvi
 *
 */
public class JsonArray
{
	public final ArrayList<Object> contents = new ArrayList<Object>();

	/**
	 * Creates an empty JSON array.
	 */
	public JsonArray()
	{
	}

	/**
	 * Creates a new JSON array with the specified contents.
	 * @param contents The array contents.
	 */
	public JsonArray(Object... contents)
	{
		for (Object object : contents) {
			this.contents.add(object);
		}
	}

	/**
	 * Adds an item to the JSON array.
	 * @param item The item.
	 */
	public void add(Object item)
	{
		this.contents.add(item);
	}

	@Override
	public String toString()
	{
		StringBuilder builder = new StringBuilder();

		builder.append("[ ");
		for (int index = 0; index < this.contents.size(); index++)
		{
			if (index > 0) builder.append(" , ");
			builder.append(JsonSerialize.serialize(this.contents.get(index)));
		}
		builder.append(" ]");

		return builder.toString();
	}
}
