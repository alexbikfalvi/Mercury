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
using System.Net;
using Newtonsoft.Json;

namespace Mercury.Api
{
    /// <summary>
    /// A class for an IP to AS mapping.
    /// </summary>
    public class MercuryIpToAsMapping
    {
        [JsonProperty("ip")]
        private string address = string.Empty;
        [JsonProperty("timeStamp")]
        private string timestamp = string.Empty;

        #region Public properties

        /// <summary>
        /// The AS number.
        /// </summary>
        [JsonProperty("as")]
        public int AsNumber{ get; private set; }
        /// <summary>
        /// The AS name.
        /// </summary>
        [JsonProperty("asName")]
	    public string AsName{ get; private set; }
        /// <summary>
        /// The address lower range.
        /// </summary>
	    [JsonProperty("rangeLow")]
        public long RangeLow{ get; private set; }
        /// <summary>
        /// The address higher range.
        /// </summary>
        [JsonProperty("rangeHigh")]
	    public long RangeHigh{ get; private set; }
        /// <summary>
        /// The addresses count.
        /// </summary>
        [JsonProperty("numIps")]
	    public long Count{ get; private set; }
        /// <summary>
        /// The address network prefix.
        /// </summary>
        [JsonProperty("prefix")]
	    public string Prefix{ get; private set; }
        /// <summary>
        /// The IXP participant.
        /// </summary>
        [JsonProperty("ixpParticipant")]
	    public int IxpParticipant{ get; private set; }
        /// <summary>
        /// The IXP participant name.
        /// </summary>
        [JsonProperty("ixpParticipantName")]
	    public string IxpParticipantName{ get; private set; }
        /// <summary>
        /// The mapping timestamp.
        /// </summary>
        public DateTime TimeStamp { get { return DateTime.Parse(this.timestamp); } }
        /// <summary>
        /// The mapping type.
        /// </summary>
        [JsonProperty("type")]
	    public string Type{ get; set; }
        /// <summary>
        /// The IP address.
        /// </summary>
        public IPAddress Address { get { return IPAddress.Parse(this.address); } }

        #endregion
    }
}
