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

import java.io.IOException;
import java.io.InputStream;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.xml.sax.SAXException;

import com.bikfalvi.java.xml.XmlExtensions;

/**
 * A class representing a culture reader.
 * @author Alex Bikfalvi
 *
 */
public class CultureReader {
	private final InputStream stream;

	/**
	 * Creates a new culture reader instance, from the specified input stream.
	 * @param stream The input stream.
	 */
	public CultureReader(InputStream stream) {
		// Validate the argument.
		if (null == stream) throw new IllegalArgumentException("stream");

		// Set the stream.
		this.stream = stream;
	}

	/**
	 * Reads a culture collection.
	 * @return The culture collection.
	 * @throws ParserConfigurationException
	 * @throws SAXException
	 * @throws IOException
	 */
	public CultureCollection readCultureCollection() throws ParserConfigurationException, SAXException, IOException {
		// Create the document builder factory.
		DocumentBuilderFactory domFactory = DocumentBuilderFactory.newInstance();
		// Create the document builder.
		DocumentBuilder domBuilder = domFactory.newDocumentBuilder();
		
		// Parse the XML document from the input stream.
		Document document = domBuilder.parse(this.stream);
		
		// Create the culture collection.
		CultureCollection cultures = new CultureCollection();

		// Parse all locale elements.
		for (Element localeElement : XmlExtensions.getElements(document.getDocumentElement(), "Locale")) {
			
			// Parse the locale culture element.
			Element cultureElement = XmlExtensions.getElement(localeElement, "Culture");
			// Create the culture.
			Culture culture = new Culture(new CultureId(
				cultureElement.getAttribute("Language"),
				cultureElement.hasAttribute("Script") ? cultureElement.getAttribute("Script") : null,
				cultureElement.hasAttribute("Territory") ? cultureElement.getAttribute("Territory") : null));

			// Get the languages element.
			Element languagesElement = XmlExtensions.getElement(localeElement, "Languages");
			// Get the scripts element.
			Element scriptsElement = XmlExtensions.getElement(localeElement, "Scripts");
			// Get the territories element.
			Element territoriesElement = XmlExtensions.getElement(localeElement, "Territories");

			// Parse the languages.
			for (Element languageElement : XmlExtensions.getElements(languagesElement, "Language")) {
				culture.getLanguages().add(languageElement.getAttribute("Type"), languageElement.getTextContent());
			}
			// Parse the scripts.
			for (Element scriptElement : XmlExtensions.getElements(scriptsElement, "Script")) {
				culture.getScripts().add(scriptElement.getAttribute("Type"), scriptElement.getTextContent());
			}
			// Parse the territories.
			for (Element territoryElement : XmlExtensions.getElements(territoriesElement, "Territory")) {
				culture.getTerritories().add(territoryElement.getAttribute("Type"), territoryElement.getTextContent());
			}

			// Add the locale.
			cultures.add(culture);
		}

		// Return the collection.
		return cultures;
	}
}

