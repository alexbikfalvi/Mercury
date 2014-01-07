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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DotNetApi.Globalization
{
	/// <summary>
	/// A class representing a collection of locale objects.
	/// </summary>
	[Serializable]
	public class LocaleCollection : IEnumerable<Locale>//, ISerializable
	{
		private readonly Dictionary<CultureId, Locale> locales;

		/// <summary>
		/// Creates an empty locale collection instance.
		/// </summary>
		public LocaleCollection()
		{
			this.locales = new Dictionary<CultureId, Locale>();
		}

		/// <summary>
		/// Creates a locale collection instance.
		/// </summary>
		/// <param name="locales">An enumeration of locales.</param>
		public LocaleCollection(IEnumerable<Locale> locales)
		{
			// Validate the arguments.
			if (null == locales) throw new ArgumentNullException("locales");

			this.locales = new Dictionary<CultureId, Locale>();
			foreach (Locale locale in locales)
			{
				this.locales.Add(locale.Culture, locale);
			}
		}

		///// <summary>
		///// Creates a locale collection from the specified serialization context.
		///// </summary>
		///// <param name="info">The serialization information.</param>
		///// <param name="context">The streaming context.</param>
		//public LocaleCollection(SerializationInfo info, StreamingContext context)
		//{
		//	this.locales = info.GetValue("locales", typeof(Dictionary<CultureId, Locale>)) as Dictionary<CultureId, Locale>;
		//}

		// Public properties.

		/// <summary>
		/// Gets the number of languages in the collection.
		/// </summary>
		public int Count
		{
			get { return this.locales.Count; }
		}

		// Public methods.

		/// <summary>
		/// Gets the enumerator for the locale collection.
		/// </summary>
		/// <returns>The collection enumerator.</returns>
		public IEnumerator<Locale> GetEnumerator()
		{
			return this.locales.Values.GetEnumerator();
		}

		/// <summary>
		/// Gets the enumerator for the locale collection.
		/// </summary>
		/// <returns>The collection enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		///// <summary>
		///// Gets the serialization data for the current object.
		///// </summary>
		///// <param name="info">The serialization info.</param>
		///// <param name="context">The streaming context.</param>
		//public void GetObjectData(SerializationInfo info, StreamingContext context)
		//{
		//	info.AddValue("locales", this.locales);
		//}

		/// <summary>
		/// Adds a new locale to the collection.
		/// </summary>
		/// <param name="locale">The locale.</param>
		public void Add(Locale locale)
		{
			// Validate the argument.
			if (null == locale) throw new ArgumentNullException("locale");
			// Add the locale.
			this.locales.Add(locale.Culture, locale);
		}

		/// <summary>
		/// Determines whether the collection contains a locale for the specified culture.
		/// </summary>
		/// <param name="culture">The culture.</param>
		public bool Contains(CultureId culture)
		{
			return this.locales.ContainsKey(culture);
		}

		/// <summary>
		/// Gets the locale for the specified culture.
		/// </summary>
		/// <param name="culture">The culture.</param>
		/// <param name="locale">The locale.</param>
		/// <returns><b>True</b> if the collection contains a locale for the specified culture, <b>false</b> otherwise.</returns>
		public bool TryGetLanguage(CultureId culture, out Locale locale)
		{
			return this.locales.TryGetValue(culture, out locale);
		}
	}
}
