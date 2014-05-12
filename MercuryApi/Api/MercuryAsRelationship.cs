/* 
 * Copyright (C) 2014 Alex Bikfalvi, Manuel Palacin
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

namespace Mercury.Api
{
    /// <summary>
    /// A class for an AS relationship.
    /// </summary>
    public class MercuryAsRelationship
    {
        /// <summary>
        /// An enumeration representing the relationship type.
        /// </summary>
        public enum RelationshipType
        {
            Customer = -1,
            Peer = 0,
            Provider = 1,
            Sibling = 3,
            Ixp = 3,
            NotFound = 10
        }

        [JsonProperty("relationship")]
        private int relationship = (int)RelationshipType.NotFound;

        /// <summary>
        /// Private constructor.
        /// </summary>
        public MercuryAsRelationship() { }

        /// <summary>
        /// Creates a new AS relationship instance.
        /// </summary>
        /// <param name="type">The relationship type.</param>
        /// <param name="asFirst">The first AS number.</param>
        /// <param name="asSecond">The second AS number.</param>
        /// <param name="hop">The hop.</param>
        public MercuryAsRelationship(
            RelationshipType type,
            int asFirst,
            int asSecond,
            byte hop)
        {
            this.Type = type;
            this.AsFirst = asFirst;
            this.AsSecond = asSecond;
            this.Hop = hop;
        }

        #region Public properties

        /// <summary>
        /// The relationship.
        /// </summary>
        public RelationshipType Type
        {
            get { return (RelationshipType)this.relationship; }
            set { this.relationship = (int)value; }
        }
        /// <summary>
        /// The first AS.
        /// </summary>
        [JsonProperty("as0")]
        public int AsFirst { get; private set; }
        /// <summary>
        /// The second AS.
        /// </summary>
        [JsonProperty("as1")]
        public int AsSecond { get; private set; }
        /// <summary>
        /// The AS hop.
        /// </summary>
        public byte Hop { get; private set; }

        #endregion
    }
}
