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
    /// A class for an IP traceroute.
    /// </summary>
    public class MercuryIpTraceroute
    {
        [JsonProperty("srcIp")]
        private string sourceAddress = string.Empty;
        [JsonProperty("srcPublicIp")]
        private string sourcePublicAddress = string.Empty;
        [JsonProperty("dstIp")]
        private string destinationAddress = string.Empty;

        /// <summary>
        /// Private constructor.
        /// </summary>
        private MercuryIpTraceroute()
        {
            this.Flows = new List<MercuryIpFlow>();
        }

        /// <summary>
        /// Creates a new IP traceroute instance.
        /// </summary>
        /// <param name="sourceAddress">The source address.</param>
        /// <param name="sourcePublicAddress">The source public address.</param>
        /// <param name="destinationAddress">The destination address.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="settingsId">The settings identifier.</param>
        public MercuryIpTraceroute(
            IPAddress sourceAddress,
            IPAddress sourcePublicAddress,
            IPAddress destinationAddress,
            string destination,
            Guid settingsId)
        {
            this.sourceAddress = sourceAddress.ToString();
            this.sourcePublicAddress = sourcePublicAddress.ToString();
            this.destinationAddress = destinationAddress.ToString();
            this.Destination = destination;
            this.SettingsId = settingsId;
            this.Flows = new List<MercuryIpFlow>();
        }

        #region Public properties

        /// <summary>
        /// The source address.
        /// </summary>
        public IPAddress SourceAddress { get { return IPAddress.Parse(this.sourceAddress); } }
        /// <summary>
        /// The source public address.
        /// </summary>
        public IPAddress SourcePublicAddress { get { return IPAddress.Parse(this.sourcePublicAddress); } }
        /// <summary>
        /// The destination address.
        /// </summary>
        public IPAddress DestinationAddress { get { return IPAddress.Parse(this.destinationAddress); } }
        /// <summary>
        /// The destination.
        /// </summary>
        [JsonProperty("dst")]
        public string Destination { get; private set; }
        /// <summary>
        /// The traceroute flows.
        /// </summary>
        [JsonProperty("tracerouteIpFlows")]
        public List<MercuryIpFlow> Flows { get; private set; }
        /// <summary>
        /// The settings identifier.
        /// </summary>
        [JsonProperty("tracerouteSettingsId")]
        public Guid SettingsId { get; private set; }

        #endregion
    }

    public class TracerouteIpAttempt
    {
        public enum State { COMPLETED, UNREACHABLE }

        public List<TracerouteIpHop> tracerouteIpHops { get; set; }
        public TracerouteIpAttempt.State state { get; set; }
        public int maxTTL { get; set; }
        public String tracerouteIpAttemptId { get; set; } //This ID must be manually generated and must match 
        //with the list of IDs of TracerouteAS.tracerouteIpAttemptIds String UUID = Guid.NewGuid().ToString();


        public TracerouteIpAttempt()
        {
            this.tracerouteIpHops = new List<TracerouteIpHop>();
        }

        public TracerouteIpAttempt(TracerouteIpAttempt.State state, int maxTTL, String tracerouteIpAttemptId)
        {
            this.state = state;
            this.maxTTL = maxTTL;
            this.tracerouteIpAttemptId = tracerouteIpAttemptId;
            this.tracerouteIpHops = new List<TracerouteIpHop>();
        }
    }

    public class TracerouteIpHop
    {
        public enum State { REQ_SENT, RESP_RECV }
        public enum Type { ECHO_REPLY, TIME_EXCEEDED, DESTINATION_UNREACHABLE, UNKNOWN }

        public TracerouteIpHop.State state { get; set; }
        public TracerouteIpHop.Type type { get; set; }
        public String ipAddr { get; set; }
        public int ttl { get; set; }
        //public DateTime reqTimeStamp{ get; set; }
        //public DateTime respTimeStamp{ get; set; }

        public TracerouteIpHop() { }

        public TracerouteIpHop(TracerouteIpHop.State state, TracerouteIpHop.Type type, String ipAddr, int ttl)
        {
            this.state = state;
            this.type = type;
            this.ipAddr = ipAddr;
            this.ttl = ttl;
        }


    }
}
