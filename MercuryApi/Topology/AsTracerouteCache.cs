/* 
 * Copyright (C) 2014 Alex Bikfalvi
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
using System.Linq;
using Mercury.Api;
using Mercury.Services;

namespace Mercury.Topology
{
	/// <summary>
	/// A class representing the cache for an AS traceroute.
	/// </summary>
	public sealed class ASTracerouteCache
	{
		private readonly ASTracerouteSettings settings;

        private readonly Dictionary<IPAddress, List<MercuryIpToAsMapping>> cache =
            new Dictionary<IPAddress, List<MercuryIpToAsMapping>>();

		/// <summary>
		/// Creates a new AS traceroute cache.
		/// </summary>
		/// <param name="settings">The settings.</param>
		public ASTracerouteCache(ASTracerouteSettings settings)
		{
			this.settings = settings;
		}

		#region Public methods

		/// <summary>
		/// Adds to the cache the information for the specified list of IP addresses.
		/// </summary>
		/// <param name="addresses">The list of IP addresses.</param>
		public void Add(IEnumerable<IPAddress> addresses)
		{

		}

		/// <summary>
		/// Gets from the cache the information for the specified IP address.
		/// </summary>
		/// <param name="address">The IP address.</param>
		/// <returns>The address information.</returns>
        public List<MercuryIpToAsMapping> Get(IPAddress address)
        {
            List<MercuryIpToAsMapping> mapping;

            // Try and get the value from the cache.
            if (this.cache.TryGetValue(address, out mapping))
            {
                // If the value is found, return the mapping.
                return mapping;
            }
            else
            {
                // Get the mapping using the Mercury service.
                mapping = MercuryService.GetIpToAsMappings(address);
                // Add the mapping to the cache.
                this.cache.Add(address, mapping);
                // Return the mapping.
                return mapping;
            }
        }

        /// <summary>
        /// Gets from the cache the information for the specified IP addresses.
        /// </summary>
        /// <param name="addresses">The list of IP addresses.</param>
        /// <returns>The address information.</returns>
        public List<List<MercuryIpToAsMapping>> Get(IEnumerable<IPAddress> addresses)
        {
            // The list of mappings.
            List<List<MercuryIpToAsMapping>> mappings = new List<List<MercuryIpToAsMapping>>();
            HashSet<IPAddress> notFoundAddresses = new HashSet<IPAddress>();

            List<MercuryIpToAsMapping> mapping;

            // Add the mappings found in the cache.
            foreach (IPAddress address in addresses)
            {
                // Try and get the value from the cache.
                if (this.cache.TryGetValue(address, out mapping))
                {
                    // If the value is found, add it to the mappings.
                    mappings.Add(mapping);
                }
                else
                {
                    // If the value is not found, add the address to the not found addresses list.
                    notFoundAddresses.Add(address);
                }
            }
            
            // Convert the hash set to a list.
            List<IPAddress> notFoundAddressesList = notFoundAddresses.ToList();

            // Request the unknown addresses to the Mercury service in groups.
            for (int index = 0, count; index < notFoundAddresses.Count; index += MercuryService.maximumAddressesPerRequest)
            {
                // Compute the number of addresses included in the current request.
                count = notFoundAddresses.Count - index > MercuryService.maximumAddressesPerRequest ?
                    MercuryService.maximumAddressesPerRequest : notFoundAddresses.Count - index;

                // Request
                foreach(List<MercuryIpToAsMapping> result in MercuryService.GetIpToAsMappings(notFoundAddressesList, index, count))
                {
                    // Add the result to the cache.
                    //this.cache.Add(result[])
                }
            }

            // Return the mappings.
            return mappings;
        }

		#endregion
	}
}
