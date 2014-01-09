/* 
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

using System;
using System.IO;
using System.Xml;

namespace DotNetApi.Globalization
{
	/// <summary>
	/// A class representing a locale reader.
	/// </summary>
	public class LocaleReader : IDisposable
	{
		private readonly TextReader reader = null;
		private readonly Stream stream = null;

		/// <summary>
		/// Creates a new reader instance, attached to the specified text reader.
		/// </summary>
		/// <param name="reader">The text reader.</param>
		public LocaleReader(TextReader reader)
		{
			// Validate the argument.
			if (null == reader) throw new ArgumentNullException("reader");

			// Set the reader.
			this.reader = reader;
		}

		/// <summary>
		/// Creates a new reader instance, attached to the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		public LocaleReader(Stream stream)
		{
			// Validate the argument.
			if (null == stream) throw new ArgumentNullException("stream");

			// Set the stream.
			this.stream = stream;
		}

		// Public methods.

		/// <summary>
		/// Disposes the current object.
		/// </summary>
		public void Dispose()
		{
			// Call the event handler.
			this.Dispose(true);
			// Supress the finalizer.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Reads a locale collection.
		/// </summary>
		/// <returns>The locale collection.</returns>
		public LocaleCollection ReadLocaleCollection()
		{
			// The XML document.
			XmlDocument document = new XmlDocument();

			// Load the XML document.
			if (null != this.reader)
				document.Load(this.reader);
			else
				document.Load(this.stream);

			// Create the locale collection.
			LocaleCollection locales = new LocaleCollection();

			// Parse all locale elements.
			foreach (XmlElement localeElement in document.DocumentElement.GetElementsByTagName("Locale"))
			{
				// Parse the locale culture element.
				XmlElement cultureElement = localeElement.GetElementsByTagName("Culture")[0] as XmlElement;
				// Create the culture.
				Locale locale = new Locale(new CultureId(
					cultureElement.GetAttribute("Language"),
					cultureElement.HasAttribute("Script") ? cultureElement.GetAttribute("Script") : null,
					cultureElement.HasAttribute("Territory") ? cultureElement.GetAttribute("Territory") : null));

				// Get the languages element.
				XmlElement languagesElement = localeElement.GetElementsByTagName("Languages")[0] as XmlElement;
				// Get the scripts element.
				XmlElement scriptsElement = localeElement.GetElementsByTagName("Scripts")[0] as XmlElement;
				// Get the territories element.
				XmlElement territoriesElement = localeElement.GetElementsByTagName("Territories")[0] as XmlElement;

				// Parse the languages.
				foreach (XmlElement languageElement in languagesElement.GetElementsByTagName("Language"))
				{
					locale.Languages.Add(languageElement.GetAttributeNode("Type").Value, languageElement.InnerText);
				}
				// Parse the scripts.
				foreach (XmlElement scriptElement in scriptsElement.GetElementsByTagName("Script"))
				{
					locale.Scripts.Add(scriptElement.GetAttributeNode("Type").Value, scriptElement.InnerText);
				}
				// Parse the territories.
				foreach (XmlElement territoryElement in territoriesElement.GetElementsByTagName("Territory"))
				{
					locale.Territories.Add(territoryElement.GetAttributeNode("Type").Value, territoryElement.InnerText);
				}

				// Add the locale.
				locales.Add(locale);
			}

			// Return the collection.
			return locales;
		}

		// Protected methods.

		/// <summary>
		/// An event handler called when the object is being disposed.
		/// </summary>
		/// <param name="disposing">If <b>true</b>, clean both managed and native resources. If <b>false</b>, clean only native resources.</param>
		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
