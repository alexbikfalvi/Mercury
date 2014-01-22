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

package edu.upf.mercury.client.resources;

import java.io.IOException;
import java.io.InputStream;
import java.util.Hashtable;
import java.util.Locale;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.xml.sax.SAXException;

import com.bikfalvi.java.xml.XmlExtensions;

/**
 * A class that loads a resources XML file.
 * @author Alex Bikfalvi
 *
 */
public final class Resources
{
	private final Hashtable<String, String> resources = new Hashtable<String, String>();
	
	/**
	 * Creates a new resources instance from the specified path.
	 * @param stream The resources stream.
	 * @throws ParserConfigurationException 
	 * @throws IOException 
	 * @throws SAXException 
	 */
	public Resources(InputStream stream) throws ParserConfigurationException, SAXException, IOException {
		// Create the document builder factory.
		DocumentBuilderFactory domFactory = DocumentBuilderFactory.newInstance();
		// Create the document builder.
		DocumentBuilder domBuilder = domFactory.newDocumentBuilder();
		
		// Parse the XML document from the input string.
		Document document = domBuilder.parse(stream);
		
		// Load the resources.
		for (Element element : XmlExtensions.getElements(document.getDocumentElement(), "data")) {
			this.resources.put(element.getAttribute("name"), XmlExtensions.getElementValue(element, "value"));
		}
	}
	
	/**
	 * Returns the resource with the specified name for the current locale.
	 * @param name The resource name.
	 * @return The resource value.
	 */
	public String get(String name) {
		String value = null;
		if (null == (value = this.resources.get(String.format("%s.%s", name, Locale.getDefault().getLanguage()))))
			value = this.resources.get(name);
		return value;
	}
	
	/**
	 * Returns the resource with the specified name for the current locale.
	 * @param name The resource name.
	 * @param mnemonic Indicates whether to strip the mnemonic ampersands.
	 * @return The resource value.
	 */
	public String get(String name, boolean mnemonic) {
		// Get the value.
		String value = null;
		if ((null != (value = this.get(name))) && mnemonic) {
			return value.replaceFirst("&", "");
		}
		else {
			return value;
		}
	}
	
	/**
	 * Gets the display index of the mnemonic character.
	 * @param name The resource name.
	 * @return The mnemonic character index, or -1 if there is no mnemonic.
	 */
	public int getMnemonic(String name) {
		// Get the value.
		String value = null;
		if (null != (value = this.get(name))) {
			int index = value.indexOf('&');
			return index < value.length() - 1 ? index : -1;
		}
		return -1;
	}
}
