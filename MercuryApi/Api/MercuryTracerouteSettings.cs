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
    /// A class for traceroute settings.
    /// </summary>
    public class MercuryTracerouteSettings
    {
        [JsonProperty("id")]
        private string id = string.Empty;

        /// <summary>
        /// Private constructor.
        /// </summary>
        public MercuryTracerouteSettings() { }

        /// <summary>
        /// Creates a new traceroute settings instance.
        /// </summary>
        /// <param name="id">The settings identifier.</param>
        /// <param name="attemptsPerFlow">The number of attempts per flow.</param>
        /// <param name="flowCount">The flow count.</param>
        /// <param name="minHops">The minimum number of hops.</param>
        /// <param name="maxHops">The maximum number of hops.</param>
        /// <param name="attemptDelay">The attempt delay in milliseconds.</param>
        /// <param name="hopTimeout">The hop timeout.</param>
        /// <param name="minPort">The minimum port.</param>
        /// <param name="maxPort">The maximum port.</param>
        /// <param name="dataLength">The data length.</param>
        public MercuryTracerouteSettings(
            Guid id,
            int attemptsPerFlow,
            int flowCount,
            int minHops,
            int maxHops,
            int attemptDelay,
            int hopTimeout,
            int minPort,
            int maxPort,
            int dataLength)
        {
            this.id = id.ToString();
            this.AttemptsPerFlow = attemptsPerFlow;
            this.FlowCount = flowCount;
            this.MinimumHops = minHops;
            this.MaximumHops = maxHops;
            this.AttemptDelay = attemptDelay;
            this.HopTimeout = hopTimeout;
            this.MinimumPort = minPort;
            this.MaximumPort = maxPort;
            this.DataLength = dataLength;
        }

        #region Public properties

        /// <summary>
        /// The identifier.
        /// </summary>
        public Guid Id { get { return Guid.Parse(this.id); } }
        /// <summary>
        /// The number of attempts per flow.
        /// </summary>
        [JsonProperty("attemptsPerFlow")]
        public int AttemptsPerFlow { get; private set; }
        /// <summary>
        /// The number of flows.
        /// </summary>
        [JsonProperty("flowCount")]
        public int FlowCount { get; private set; }
        /// <summary>
        /// The minimum number of hops.
        /// </summary>
        [JsonProperty("minHops")]
        public int MinimumHops { get; private set; }
        /// <summary>
        /// The maximum number of hops.
        /// </summary>
        [JsonProperty("maxHops")]
        public int MaximumHops { get; private set; }
        /// <summary>
        /// The delay between attempts in milliseconds.
        /// </summary>
        [JsonProperty("attemptDelay")]
        public int AttemptDelay { get; private set; }
        /// <summary>
        /// The hop timeout in milliseconds.
        /// </summary>
        [JsonProperty("hopTimeout")]
        public int HopTimeout { get; private set; }
        /// <summary>
        /// The minimum UDP port.
        /// </summary>
        [JsonProperty("minPort")]
        public int MinimumPort { get; private set; }
        /// <summary>
        /// The maximum UDP port.
        /// </summary>
        [JsonProperty("maxPort")]
        public int MaximumPort { get; private set; }
        /// <summary>
        /// The data length in bytes.
        /// </summary>
        [JsonProperty("dataLength")]
        public int DataLength { get; private set; }

        #endregion
    }
}
