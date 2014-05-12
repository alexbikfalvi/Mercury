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

        public MercuryAsTracerouteRelationship() { }

        public MercuryAsTracerouteRelationship(RelationshipType relationship, int as0, int as1, int hop)
        {
            this.relationship = relationship;
            this.as0 = as0;
            this.as1 = as1;
            this.hop = hop;
        }

        #region Public properties

        [JsonConverter(typeof(JsonEnumConverter))]
        public RelationshipType relationship { get; set; }
        public int as0 { get; set; }
        public int as1 { get; set; }
        public int hop { get; set; }

        #endregion
    }
}
