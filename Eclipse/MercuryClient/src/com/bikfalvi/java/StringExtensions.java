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

package com.bikfalvi.java;

public final class StringExtensions {
	/**
	 * Compares two string values for equality.
	 * @param left The left string.
	 * @param right The right string.
	 * @return True if the two strings are equal, false otherwise.
	 */
	public static boolean equals(String left, String right) {
		if ((null == left) && (null == right)) return true;
		else if (null == left) return false;
		else if (null == right) return false;
		else return left.equals(right);
	}
	
	/**
	 * Compares two string values for equality, ignoring the case.
	 * @param left The left string.
	 * @param right The right string.
	 * @return True if the two strings are equal, false otherwise.
	 */
	public static boolean equalsIgnoreCase(String left, String right) {
		if ((null == left) && (null == right)) return true;
		else if (null == left) return false;
		else if (null == right) return false;
		else return left.equalsIgnoreCase(right);
	}
}
