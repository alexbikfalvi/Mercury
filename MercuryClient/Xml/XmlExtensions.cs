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

using System;
using System.Collections;
using System.Xml;

namespace Mercury.Xml
{
	/// <summary>
	/// A class with method for XML for .NET 2.0.
	/// </summary>
	public static class XmlExtensions
	{
		/// <summary>
		/// Returns an enumeration of XML elements with the specified name.
		/// </summary>
		/// <param name="element">The parent XML element.</param>
		/// <param name="name">The name of child XML elements.</param>
		/// <returns>The enumeration of child XML elements.</returns>
		public static IEnumerable GetElements(XmlElement element, string name)
		{
			return element.GetElementsByTagName(name);
		}

		/// <summary>
		/// Returns the first child XML element with the specified name.
		/// </summary>
		/// <param name="element">The parent XML element.</param>
		/// <param name="name">The name of the child XML element.</param>
		/// <returns>The child XML element if exists, or <b>null</b> otherwise.</returns>
		public static XmlElement GetElement(XmlElement element, string name)
		{
			XmlNodeList children = element.GetElementsByTagName(name);
			return children.Count > 0 ? children[0] as XmlElement : null;
		}

		/// <summary>
		/// Returns the value of the first child XML element with the specified name.
		/// </summary>
		/// <param name="element">The parent XML element.</param>
		/// <param name="name">The name of the child XML element.</param>
		/// <returns>The value.</returns>
		public static string GetElementValue(XmlElement element, string name)
		{
			XmlElement child = XmlExtensions.GetElement(element, name);
			return child != null ? child.InnerText : null;
		}
	}
}
