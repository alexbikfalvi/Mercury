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
using Mercury.Topology;

namespace Mercury.Api
{
    /// <summary>
    /// A class for a Mercury traceroute hop.
    /// </summary>
    public class MercuryAsTracerouteHop
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        public MercuryAsTracerouteHop() { }

        /// <summary>
        /// Creates a new traceroute hop instance for missing hops.
        /// </summary>
        /// <param name="hop">The hop index.</param>
        public MercuryAsTracerouteHop(byte hop) 
        {
            this.Hop = hop;
        }

        /// <summary>
        /// Creates a new traceroute hop instance.
        /// </summary>
        /// <param name="hop">The hop index.</param>
        /// <param name="infos">The AS information.</param>
 //       /// <param name="isInferred">Indicates whether the hop AS is inferred.</param>
        public MercuryAsTracerouteHop(byte hop, ASInformation info/*, bool isInferred*/)
        {
            this.Hop = hop;
            this.AsNumber = info.AsNumber;
            this.AsName = info.AsName;
            this.IxpName = info.IxpName;
            this.Type = info.Type;
            //this.IsInferred = isInferred;
        }

        #region Public properties

        /// <summary>
        /// The hop index.
        /// </summary>
        [JsonProperty("hop")]
        public byte Hop { get; private set; }
        /// <summary>
        /// The AS number.
        /// </summary>
        [JsonProperty("as")]
        public int AsNumber { get; private set; }
        /// <summary>
        /// The AS name.
        /// </summary>
        [JsonProperty("asName")]
        public string AsName { get; private set; }
        /// <summary>
        /// The IXP name.
        /// </summary>
        [JsonProperty("ixpName")]
        public string IxpName { get; private set; }
        /// <summary>
        /// The hop type.
        /// </summary>
        [JsonConverter(typeof(JsonEnumConverter))]
        [JsonProperty("type")]
        public ASInformation.AsType Type { get; private set; }
        ///// <summary>
        ///// Indicates if we use an heuristic to determine what is the most suitable AS.
        ///// </summary>
        //[JsonProperty("inferred")]
        //public bool IsInferred { get; private set; }

        #endregion
    }
}
