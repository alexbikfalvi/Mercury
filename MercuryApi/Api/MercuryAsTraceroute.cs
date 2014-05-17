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
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mercury.Api
{
    /// <summary>
    /// A class representing a Mercury AS traceroute.
    /// </summary>
    public class MercuryAsTraceroute
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private MercuryAsTraceroute()
        {
            this.Attempts = new List<string>();
            this.Hops = new List<MercuryAsTracerouteHop>();
            this.Relationships = new List<MercuryAsTracerouteRelationship>();
        }

        /// <summary>
        /// Creates a new Mercury AS traceroute.
        /// </summary>
        /// <param name="srcAS"></param>
        /// <param name="srcASName"></param>
        /// <param name="srcIp"></param>
        /// <param name="srcPublicIp"></param>
        /// <param name="srcCity"></param>
        /// <param name="srcCountry"></param>
        /// <param name="dstAS"></param>
        /// <param name="dstASName"></param>
        /// <param name="dstIp"></param>
        /// <param name="dst"></param>
        /// <param name="dstCity"></param>
        /// <param name="dstCountry"></param>
        /// <param name="timeStamp"></param>
        /// <param name="tracerouteASStats"></param>
        public MercuryAsTraceroute(
            int srcAS, string srcASName, string srcIp,
            string srcPublicIp, string srcCity, string srcCountry, int dstAS,
            string dstASName, string dstIp, string dst, string dstCity,
            string dstCountry, DateTime timeStamp, MercuryAsTracerouteStats tracerouteASStats)
        {
            this.SourceAsNumber = srcAS;
            this.SourceAsName = srcASName;
            this.SourceAddress = srcIp;
            this.SourcePublicAddress = srcPublicIp;
            this.SourceCity = srcCity;
            this.SourceCountry = srcCountry;
            this.DestinationAsNumber = dstAS;
            this.DestinationAsName = dstASName;
            this.DestinationAddress = dstIp;
            this.Destination = dst;
            this.DestinationCity = dstCity;
            this.DestinationCountry = dstCountry;
            this.Timestamp = timeStamp;
            this.Statistics = tracerouteASStats;

            this.Attempts = new List<string>();
            this.Hops = new List<MercuryAsTracerouteHop>();
            this.Relationships = new List<MercuryAsTracerouteRelationship>();
        }

        #region Public properties

        [JsonProperty("srcAS")]
        public int SourceAsNumber { get; private set; }
        [JsonProperty("srcASName")]
        public string SourceAsName { get; private set; }
        [JsonProperty("srcIp")]
        public string SourceAddress { get; private set; }
        [JsonProperty("srcPublicIp")]
        public string SourcePublicAddress { get; private set; }
        [JsonProperty("srcCity")]
        public string SourceCity { get; private set; }
        [JsonProperty("srcCountry")]
        public string SourceCountry { get; private set; }
        [JsonProperty("dstAS")]
        public int DestinationAsNumber { get; private set; }
        [JsonProperty("dstASName")]
        public string DestinationAsName { get; private set; }
        [JsonProperty("dstIp")]
        public string DestinationAddress { get; private set; }
        [JsonProperty("dst")]
        public string Destination { get; private set; }
        [JsonProperty("dstCity")]
        public string DestinationCity { get; private set; }
        [JsonProperty("dstCountry")]
        public string DestinationCountry { get; private set; }
        [JsonProperty("timeStamp")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; private set; }
        [JsonProperty("tracerouteIpAttemptIds")]
        public List<string> Attempts { get; private set; }
        [JsonProperty("tracerouteASHops")]
        public List<MercuryAsTracerouteHop> Hops { get; private set; }
        [JsonProperty("tracerouteASRelationships")]
        public List<MercuryAsTracerouteRelationship> Relationships { get; private set; }
        [JsonProperty("tracerouteASStats")]
        public MercuryAsTracerouteStats Statistics { get; private set; }

        #endregion
    }
}
