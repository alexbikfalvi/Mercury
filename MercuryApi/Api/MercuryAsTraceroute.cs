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
using System.Net;
using Newtonsoft.Json;

namespace Mercury.Api
{
    /// <summary>
    /// A class for an AS traceroute.
    /// </summary>
    public class MercuryAsTraceroute
    {
        [JsonProperty("srcIp")]
        private string sourceAddress;
        [JsonProperty("srcPublicIp")]
        private string sourcePublicAddress;
        [JsonProperty("dstIp")]
        private string destinationAddress;
        [JsonProperty("timeStamp")]
        private string timestamp;

        /// <summary>
        /// Private constructor.
        /// </summary>
        private MercuryAsTraceroute()
        {
            this.Attempts = new List<string>();
            this.Hops = new List<MercuryAsTracerouteHop>();
            this.Relationships = new List<MercuryAsRelationship>();
        }

        /// <summary>
        /// Creates a new AS traceroute instance.
        /// </summary>
        /// <param name="sourceAsNumber">The source AS number.</param>
        /// <param name="sourceAsName">The source AS name.</param>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="sourcePublicAdress">The source public address.</param>
        /// <param name="sourceCity">The source city.</param>
        /// <param name="sourceCountry">The source country.</param>
        /// <param name="destinationAsNumbers">The destination AS number.</param>
        /// <param name="destinationAsName">The destination AS name.</param>
        /// <param name="destinationAddress">The destination address.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="destinationCity">The destination city.</param>
        /// <param name="destinationCountry">The destination country.</param>
        /// <param name="statistics">The statistics.</param>
        public MercuryAsTraceroute(
            int sourceAsNumber,
            string sourceAsName,
            IPAddress sourceAddress,
            IPAddress sourcePublicAddress,
            string sourceCity,
            string sourceCountry,
            int destinationAsNumber,
            string destinationAsName,
            IPAddress destinationAddress,
            string destination,
            string destinationCity,
            string destinationCountry,
            /* DateTime timeStamp,*/
            MercuryAsTracerouteStatistics statistics)
        {
            this.SourceAsNumber = sourceAsNumber;
            this.SourceAsName = sourceAsName;
            this.SourceAddress = sourceAddress;
            this.SourcePublicAddress = sourcePublicAddress;
            this.SourceCity = sourceCity;
            this.SourceCountry = sourceCountry;
            this.DestinationAsNumber = destinationAsNumber;
            this.DestinationAsName = destinationAsName;
            this.DestinationAddress = destinationAddress;
            this.Destination = destination;
            this.DestinationCity = destinationCity;
            this.DestinationCountry = destinationCountry;
            //this.timeStamp = timeStamp;
            this.Statistics = statistics;

            this.Attempts = new List<string>();
            this.Hops = new List<MercuryAsTracerouteHop>();
            this.Relationships = new List<MercuryAsRelationship>();
        }

        #region Public properties

        /// <summary>
        /// The source AS number.
        /// </summary>
        [JsonProperty("srcAS")]
        public int SourceAsNumber { get; private set; }
        /// <summary>
        /// The source AS name.
        /// </summary>
        [JsonProperty("srcASName")]
        public string SourceAsName { get; private set; }
        /// <summary>
        /// The source IP address.
        /// </summary>
        public IPAddress SourceAddress
        {
            get { return IPAddress.Parse(this.sourceAddress); }
            set { this.sourceAddress = value.ToString(); }
        }
        /// <summary>
        /// The source public IP address.
        /// </summary>
        public IPAddress SourcePublicAddress
        {
            get { return IPAddress.Parse(this.sourcePublicAddress); }
            set { this.sourcePublicAddress = value.ToString(); }
        }
        /// <summary>
        /// The source city.
        /// </summary>
        [JsonProperty("srcCity")]
        public String SourceCity { get; private set; }
        /// <summary>
        /// The the source country.
        /// </summary>
        [JsonProperty("srcCountry")]
        public String SourceCountry { get; private set; }
        /// <summary>
        /// The destination AS number.
        /// </summary>
        [JsonProperty("dstAS")]
        public int DestinationAsNumber { get; private set; }
        /// <summary>
        /// The destination AS name.
        /// </summary>
        [JsonProperty("dstASName")]
        public string DestinationAsName { get; private set; }
        /// <summary>
        /// The destination address.
        /// </summary>
        public IPAddress DestinationAddress
        {
            get { return IPAddress.Parse(this.destinationAddress); }
            set { this.destinationAddress = value.ToString(); }
        }
        /// <summary>
        /// The destination.
        /// </summary>
        [JsonProperty("dst")]
        public string Destination { get; private set; }
        /// <summary>
        /// The destination city.
        /// </summary>
        [JsonProperty("dstCity")]
        public string DestinationCity { get; private set; }
        /// <summary>
        /// The destination country.
        /// </summary>
        [JsonProperty("dstCountry")]
        public String DestinationCountry { get; private set; }
        /// <summary>
        /// The traceroute timestamp.
        /// </summary>
        public DateTime Timestamp { get { return DateTime.Parse(this.timestamp); } }
        /// <summary>
        /// The traceroute attempts identifiers.
        /// </summary>
        [JsonProperty("tracerouteIpAttemptIds")]
        public List<string> Attempts { get; private set; }
        /// <summary>
        /// The traceroute hops.
        /// </summary>
        [JsonProperty("tracerouteASHops")]
        public List<MercuryAsTracerouteHop> Hops { get; private set; }
        /// <summary>
        /// The traceroute relationships.
        /// </summary>
        [JsonProperty("tracerouteASRelationships")]
        public List<MercuryAsRelationship> Relationships { get; private set; }
        /// <summary>
        /// The traceroute statistics.
        /// </summary>
        [JsonProperty("tracerouteASStats")]
        public MercuryAsTracerouteStatistics Statistics { get; private set; }

        #endregion
    }
}
