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
    /// A class for the traceroute settings.
    /// </summary>
    public class MercuryTracerouteSettings
    {
        public MercuryTracerouteSettings(String id, int attemptsPerFlow, int flowCount, int minHops, int maxHops, int attemptDelay, int hopTimeout, int minPort, int maxPort, int dataLength)
        {
            this.id = id;
            this.attemptsPerFlow = attemptsPerFlow;
            this.flowCount = flowCount;
            this.minHops = minHops;
            this.maxHops = maxHops;
            this.attemptDelay = attemptDelay;
            this.hopTimeout = hopTimeout;
            this.minPort = minPort;
            this.maxPort = maxPort;
            this.dataLength = dataLength;
        }

        #region Public properties

        public String id { get; set; }
        public int attemptsPerFlow { get; set; }
        public int flowCount { get; set; }
        public int minHops { get; set; }
        public int maxHops { get; set; }
        public int attemptDelay { get; set; }
        public int hopTimeout { get; set; }
        public int minPort { get; set; }
        public int maxPort { get; set; }
        public int dataLength { get; set; }

        #endregion
    }
}
