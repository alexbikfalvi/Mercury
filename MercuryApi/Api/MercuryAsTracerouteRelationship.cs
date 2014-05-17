/* 
 * Copyright (C) 2014 Manuel Palacin, Alex Bikfalvi
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
using Mercury.Json;

namespace Mercury.Api
{
    /// <summary>
    /// A class for an AS traceroute relationship.
    /// </summary>
    public class MercuryAsTracerouteRelationship
    {
        /// <summary>
        /// An enumeration for the relationship type.
        /// </summary>
        public enum RelationshipType
        {
            [JsonEnum("C2P")]
            CustomerToProvider,
            [JsonEnum("P2P")]
            PeerToPeer,
            [JsonEnum("P2C")]
            ProviderToCustomer,
            [JsonEnum("S2S")]
            SiblingToSibling,
            [JsonEnum("IXP")]
            InternerExchangePoint,
            [JsonEnum("NF")]
            NotFound
        };

        /// <summary>
        /// Private constructor.
        /// </summary>
        public MercuryAsTracerouteRelationship() { }

        /// <summary>
        /// Creates a new AS relationship.
        /// </summary>
        /// <param name="relationship">The relationship type.</param>
        /// <param name="asFirst">The first AS.</param>
        /// <param name="asSecond">The second AS.</param>
        /// <param name="hop">The hop index.</param>
        public MercuryAsTracerouteRelationship(RelationshipType relationship, int asFirst, int asSecond, byte hop)
        {
            this.Relationship = relationship;
            this.AsFirst = asFirst;
            this.AsSecond = asSecond;
            this.Hop = hop;
        }

        #region Public properties

        /// <summary>
        /// The relationship.
        /// </summary>
        [JsonConverter(typeof(JsonEnumConverter))]
        [JsonProperty("relationship")]
        public RelationshipType Relationship { get; private set; }
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
        /// The hop index.
        /// </summary>
        [JsonProperty("hop")]
        public byte Hop { get; set; }

        #endregion
    }
}
