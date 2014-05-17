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

namespace Mercury.Api
{
    /// <summary>
    /// A class with AS traceroute statistics.
    /// </summary>
    public class MercuryAsTracerouteStats
    {
        /// <summary>
        /// Provate constructor.
        /// </summary>
        public MercuryAsTracerouteStats() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asHops"></param>
        /// <param name="c2pRels"></param>
        /// <param name="p2pRels"></param>
        /// <param name="p2cRels"></param>
        /// <param name="s2sRels"></param>
        /// <param name="ixpRels"></param>
        /// <param name="nfRels"></param>
        /// <param name="isCompleted"></param>
        /// <param name="flags"></param>
        public MercuryAsTracerouteStats(
            int asHops,
            int c2pRels,
            int p2pRels,
            int p2cRels,
            int s2sRels,
            int ixpRels,
            int nfRels,
            bool isCompleted,
            int flags)
        {
            this.HopCount = asHops;
            this.CustomerToProviderRelationships = c2pRels;
            this.PeerToPeerRelationships = p2pRels;
            this.ProviderToCustomerRelationships = p2cRels;
            this.SiblingToSiblingRels = s2sRels;
            this.IxpRelationships = ixpRels;
            this.NotFoundRelationships = nfRels;
            this.IsCompleted = isCompleted;
            this.Flags = flags;
        }

        #region Public properties

        [JsonProperty("asHops")]
        public int HopCount { get; private set; }
        [JsonProperty("c2pRels")]
        public int CustomerToProviderRelationships { get; private set; }
        [JsonProperty("p2pRels")]
        public int PeerToPeerRelationships { get; private set; }
        [JsonProperty("p2cRels")]
        public int ProviderToCustomerRelationships { get; private set; }
        [JsonProperty("s2sRels")]
        public int SiblingToSiblingRels { get; private set; }
        [JsonProperty("ixpRels")]
        public int IxpRelationships { get; private set; }
        [JsonProperty("nfRels")]
        public int NotFoundRelationships { get; private set; }
        [JsonProperty("completed")]
        public bool IsCompleted { get; private set; }
        [JsonProperty("flags")]
        public int Flags { get; private set; }

        #endregion
    }
}
