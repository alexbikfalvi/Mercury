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
using Mercury.Api;
using Mercury.Json;

namespace Mercury.Topology
{
    /// <summary>
    /// A class that stores AS information.
    /// </summary>
    public class ASInformation
    {
        /// <summary>
        /// The AS type.
        /// </summary>
        public enum AsType
        {
            [JsonEnum("AS")]
            As,
            [JsonEnum("IXP")]
            Ixp
        };

        /// <summary>
        /// Creates a new AS information from the specified IP to AS mapping.
        /// </summary>
        /// <param name="mapping">The IP to AS mapping.</param>
        public ASInformation(MercuryIpToAsMapping mapping)
        {
            this.AsNumber = mapping.AsNumber;
            this.AsName = mapping.AsName;
            this.RangeLow = mapping.RangeLow;
            this.RangeHigh = mapping.RangeHigh;
            this.Count = mapping.Count;
            this.Prefix = mapping.Prefix;
            this.IxpName = mapping.IxpName;
            this.Timestamp = mapping.Timestamp;
            this.Type = mapping.Type;
        }

        #region Public properties

        /// <summary>
        /// The AS number.
        /// </summary>
        public int AsNumber { get; private set; }
        /// <summary>
        /// The AS name.
        /// </summary>
        public string AsName { get; private set; }
        /// <summary>
        /// The lower boundary of the IPv4 address range.
        /// </summary>
        public uint RangeLow { get; private set; }
        /// <summary>
        /// The upper boundary of the IPv4 address range.
        /// </summary>
        public uint RangeHigh { get; private set; }
        /// <summary>
        /// The number of IP addresses.
        /// </summary>
        public uint Count { get; private set; }
        /// <summary>
        /// The subnetwork prefix.
        /// </summary>
        public string Prefix { get; private set; }
        /// <summary>
        /// The IXP name.
        /// </summary>
        public string IxpName { get; private set; }
        /// <summary>
        /// The information timestamp.
        /// </summary>
        public DateTime Timestamp { get; private set; }
        /// <summary>
        /// The AS type.
        /// </summary>
        public AsType Type { get; private set; }

        #endregion
    }
}
