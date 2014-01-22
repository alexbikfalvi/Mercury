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

package com.bikfalvi.java.xml;

import java.util.Iterator;
import java.util.NoSuchElementException;

import org.w3c.dom.Element;
import org.w3c.dom.NodeList;

/**
 * A class with method for XML.
 * @author Alex Bikfalvi
 *
 */
public class XmlExtensions
{
	/**
	 * Returns an iteration of XML elements with the specified name.
	 * @param element The parent XML element.
	 * @param name The name of child XML elements.
	 * @return The iteration of child XML elements.
	 */
	public static Iterable<Element> getElements(Element element, String name)
	{
		// Get the elements.
		final NodeList nodes = element.getElementsByTagName(name);

		// Return a new iterable for the elements.
		return new Iterable<Element>() {

			@Override
			public Iterator<Element> iterator() {
				return new Iterator<Element>() {
					
					private int index = 0;

					@Override
					public boolean hasNext() {
						return index < nodes.getLength();
					}

					@Override
					public Element next() {
						if (this.hasNext())
							return (Element) nodes.item(index++);
						else
							throw new NoSuchElementException();
					}

					@Override
					public void remove() {
						throw new UnsupportedOperationException();
					}			
				};
			}
		};
	}

	/**
	 * Returns the first child XML element with the specified name.
	 * @param element The parent XML element.
	 * @param name The name of the child XML element.
	 * @return The child XML element if exists, or null otherwise.
	 */
	public static Element getElement(Element element, String name)
	{
		// Get the elements.
		final NodeList nodes = element.getElementsByTagName(name);
		
		return nodes.getLength() > 0 ? (Element) nodes.item(0) : null;
	}

	/**
	 * Returns the value of the first child XML element with the specified name.
	 * @param element The parent XML element.
	 * @param name The name of the child XML element.
	 * @return The value.
	 */
	public static String getElementValue(Element element, String name)
	{
		Element child = XmlExtensions.getElement(element, name);
		return child != null ? child.getTextContent() : null;
	}
}
