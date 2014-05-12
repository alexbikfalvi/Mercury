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
    /// A class representing an AS traceroute hop.
    /// </summary>
    public class MercuryAsTracerouteHop
    {
        /// <summary>
        /// An enumeration representing the AS hop type.
        /// </summary>
        public enum HopType
        {
            As,
            Ixp
        };

        /// <summary>
        /// Private constructor.
        /// </summary>
        private MercuryAsTracerouteHop() { }

        /// <summary>
        /// Creates a new AS traceroute hop.
        /// </summary>
        /// <param name="hop">The hop number.</param>
        /// <param name="asNumber">The AS number.</param>
        /// <param name="asName">The AS name.</param>
        /// <param name="ixpName">The IXP name.</param>
        /// <param name="type">The hop type.</param>
        /// <param name="isInferred">Indicates whether the AS was determined using an heuristic.</param>
        public MercuryAsTracerouteHop(byte hop, int asNumber, string asName, string ixpName, HopType type, bool isInferred)
        {
            this.Hop = hop;
            this.AsNumber = asNumber;
            this.AsName = asName;
            this.IxpName = ixpName;
            this.Type = type;
            this.IsInferred = isInferred;
        }

        #region Public properties

        /// <summary>
        /// The hop index.
        /// </summary>
        public byte Hop { get; private set; }
        /// <summary>
        /// The AS number.
        /// </summary>
        [JsonProperty("as")]
        public int AsNumber { get; private set; }
        /// <summary>
        /// The AS name.
        /// </summary>
        public string AsName { get; private set; }
        /// <summary>
        /// The IXP name.
        /// </summary>
        public string IxpName { get; private set; }
        /// <summary>
        /// The hop type.
        /// </summary>
        public HopType Type { get; private set; }
        /// <summary>
        /// Indicates whether the AS was determined using an heuristic.
        /// </summary>
        public bool IsInferred { get; private set; }

        #endregion
    }
}
