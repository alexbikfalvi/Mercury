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
using Newtonsoft.Json;
using DotNetApi;

namespace Mercury.Json
{
    /// <summary>
    /// A JSON converted for an enumerable.
    /// </summary>
    public class JsonEnumConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Validate the argument.
            if (!(value is Enum)) throw new ArgumentException("The object value must be an enum.");

            Enum enumValue = value as Enum;

            // If the enumeration value has a JSON attribute.
            if (enumValue.HasAttribute<JsonEnumAttribute>())
            {
                writer.WriteValue(enumValue.GetAttribute<JsonEnumAttribute>().Name);
            }
            else
            {
                writer.WriteValue(enumValue.ToString());
            }
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Validate the type.
            if (!objectType.IsEnum) throw new ArgumentException("The object type must be enum.");

            // Get the enum string.
            string enumString = reader.Value as string;

            // For all the enum values.
            foreach (Enum value in Enum.GetValues(objectType))
            {
                if (value.HasAttribute<JsonEnumAttribute>())
                {
                    if (value.GetAttribute<JsonEnumAttribute>().Name == enumString)
                    {
                        return value;
                    }
                }
                if (value.ToString() == enumString)
                {
                    return value;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}
