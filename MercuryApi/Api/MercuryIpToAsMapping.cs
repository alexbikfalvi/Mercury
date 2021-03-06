﻿/* 
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
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Mercury.Json;
using Mercury.Topology;

namespace Mercury.Api
{
    /// <summary>
    /// A class for an IP to AS mapping.
    /// </summary>
    public class MercuryIpToAsMapping
    {
        [JsonProperty("ip")]
        private string address = IPAddress.Any.ToString();

        #region Public properties

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
        /// The lower boundary of the IPv4 address range.
        /// </summary>
	    [JsonProperty("rangeLow")]
        public uint RangeLow { get; private set; }
        /// <summary>
        /// The upper boundary of the IPv4 address range.
        /// </summary>
        [JsonProperty("rangeHigh")]
	    public uint RangeHigh { get; private set; }
        /// <summary>
        /// The number of IP addresses.
        /// </summary>
        [JsonProperty("numIps")]
	    public uint Count { get; private set; }
        /// <summary>
        /// The subnetwork prefix.
        /// </summary>
        [JsonProperty("prefix")]
	    public string Prefix { get; private set; }
        /// <summary>
        /// The IXP name.
        /// </summary>
        [JsonProperty("ixpName")]
	    public string IxpName { get; private set; }
        /// <summary>
        /// The information timestamp.
        /// </summary>
        [JsonProperty("timeStamp")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; private set; }
        /// <summary>
        /// The mapping type.
        /// </summary>
        [JsonProperty("type")]
        [JsonConverter(typeof(JsonEnumConverter))]
        public ASInformation.AsType Type { get; private set; }
        /// <summary>
        /// The mapping request address.
        /// </summary>
        public IPAddress Address { get { return IPAddress.Parse(this.address); } }

        #endregion
    }
}
