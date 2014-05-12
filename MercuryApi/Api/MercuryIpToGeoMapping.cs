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
using System.Net;
using Newtonsoft.Json;

namespace Mercury.Api
{
    /// <summary>
    /// A class for an IP to geoinformation mapping.
    /// </summary>
    public class MercuryIpToGeoMapping
    {
        [JsonProperty("ip")]
        private string address = IPAddress.Any.ToString();

        #region Public properties

        /// <summary>
        /// The IP address.
        /// </summary>
        public IPAddress Address { get { return IPAddress.Parse(this.address); } }
        [JsonProperty("countryCode")]
        /// The country code.
        public string CountryCode { get; private set; }
        /// <summary>
        /// The country name.
        /// </summary>
        [JsonProperty("countryName")]
        public string CountryName { get; set; }
        /// <summary>
        /// The city.
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        #endregion
    }
}
