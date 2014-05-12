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
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mercury.Api
{
    /// <summary>
    /// A class representing a flow for an IP traceroute.
    /// </summary>
    public class MercuryIpFlow
    {
        /// <summary>
        /// An enumeration for the multipath traceroute flow algorithm.
        /// </summary>
        public enum AlgorithmType
        {
            Icmp,
            UdpIdentification,
            UdpChecksum,
            UdpBoth
        }

        [JsonProperty("flowId")]
        private string id = string.Empty;

        /// <summary>
        /// Private constructor.
        /// </summary>
        private MercuryIpFlow()
        {
            this.Attempts = new List<TracerouteIpAttempt>();
        }

        /// <summary>
        /// Creates a new flow for a multipath traceroute.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="algorithm">The algorithm.</param>
        public MercuryIpFlow(Guid id, AlgorithmType algorithm)
        {
            this.id = id.ToString();
            this.Algorithm = algorithm;
            this.Attempts = new List<TracerouteIpAttempt>();
        }

        /// <summary>
        /// The flow identifier.
        /// </summary>
        public Guid Id { get { return Guid.Parse(this.id); } }
        /// <summary>
        /// The algorithm type.
        /// </summary>
        [JsonProperty("algorithm")]
        public AlgorithmType Algorithm { get; private set; }
        /// <summary>
        /// The flow attempts.
        /// </summary>
        [JsonProperty("tracerouteIpAttemps")]
        public List<TracerouteIpAttempt> Attempts { get; private set; }
    }
}
