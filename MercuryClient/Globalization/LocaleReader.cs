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

				// Parse the locale territories.
				foreach (XmlElement territoryElement in (localeElement.GetElementsByTagName("Territories")[0] as XmlElement).GetElementsByTagName()

				// Add the locale.
				locales.Add(locale);
			}

			//// Create the locale collection.
			//LocaleCollection locales = new LocaleCollection(
			//	document.Root.Elements("Locale").Select((XElement localeElement) =>
			//		{
			//			XElement cultureElement = localeElement.Element("Culture");
			//			XAttribute languageAttribute = cultureElement.Attribute("Language");
			//			XAttribute scriptAttribute = cultureElement.Attribute("Script");
			//			XAttribute territoryAttribute = cultureElement.Attribute("Territory");
			//			return new Locale(new CultureId(
			//				languageAttribute.Value,
			//				scriptAttribute != null ? scriptAttribute.Value : null,
			//				territoryAttribute != null ? territoryAttribute.Value : null
			//				),
			//				localeElement.Element("Languages").Elements("Language").Select((XElement languageElement) =>
			//					{
			//						return new Language(languageElement.Attribute("Type").Value, languageElement.Value);
			//					}),
			//				localeElement.Element("Scripts").Elements("Script").Select((XElement scriptElement) =>
			//					{
			//						return new Script(scriptElement.Attribute("Type").Value, scriptElement.Value);
			//					}),
			//				localeElement.Element("Territories").Elements("Territory").Select((XElement territoryElement) =>
			//					{
			//						return new Territory(territoryElement.Attribute("Type").Value, territoryElement.Value);
			//					}));
			//		}));

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
