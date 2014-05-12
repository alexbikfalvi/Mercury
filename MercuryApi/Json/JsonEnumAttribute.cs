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

namespace Mercury.Json
{
    /// <summary>
    /// A JSON attribute for enumerable members.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple=false)]
    public class JsonEnumAttribute : Attribute
    {
        /// <summary>
        /// Creates a new JSON enum attribute with the specified name.
        /// </summary>
        /// <param name="name">The JSON name.</param>
        public JsonEnumAttribute(string name)
        {
            this.Name = name;
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the field JSON name.
        /// </summary>
        public string Name { get; private set; }

        #endregion
    }
}
