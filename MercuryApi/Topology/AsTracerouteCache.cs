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
using InetApi.Net;
using Mercury.Api;
using Mercury.Services;

namespace Mercury.Topology
{
	/// <summary>
	/// A class representing the cache for an AS traceroute.
	/// </summary>
	public sealed class ASTracerouteCache
	{
        private readonly object sync = new object();

		private readonly ASTracerouteSettings settings;

        private readonly Dictionary<IPAddress, List<ASInformation>> ipMapping =
            new Dictionary<IPAddress, List<ASInformation>>();
        private readonly Dictionary<int, ASInformation> asMapping =
            new Dictionary<int, ASInformation>();

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
		/// Gets from the cache the information for the specified IP address.
		/// </summary>
		/// <param name="address">The IP address.</param>
		/// <returns>The address information.</returns>
        public List<ASInformation> Get(IPAddress address)
        {
            List<ASInformation> list;

            lock (this.sync)
            {
                // Try and get the value from the cache.
                if (this.ipMapping.TryGetValue(address, out list))
                {
                    // If the value is found, return the mapping.
                    return list;
                }
            }

            // Create a new list.
            list = new List<ASInformation>();
            // Get the mapping using the Mercury service.
            foreach (MercuryIpToAsMapping mapping in MercuryService.GetIpToAsMappings(address))
            {
                // Create a new AS information.
                ASInformation info = new ASInformation(mapping);
                // Add the AS information to the list.
                list.Add(info);
            }

            lock (this.sync)
            {
                foreach (ASInformation info in list)
                {
                    // Add the AS to the cache.
                    this.asMapping[info.AsNumber] = info;
                }
                // Add the address to the cache.
                this.ipMapping[address] = list;
            }
            // Return the mapping.
            return list;
        }

        /// <summary>
        /// Updates the cache with the specified IP addresses.
        /// </summary>
        /// <param name="addresses">The list of IP addresses.</param>
        /// <returns>The address information.</returns>
        public void Update(IEnumerable<IPAddress> addresses)
        {
            // The list of not found IP addresses.
            HashSet<IPAddress> notFoundAddresses = new HashSet<IPAddress>();

            lock (this.sync)
            {
                // Add the mappings found in the cache.
                foreach (IPAddress address in addresses)
                {
                    // If the IP address is private
                    if (!address.IsGlobalUnicastAddress()) continue;

                    // Check if the address is contained in the cache.
                    if (this.ipMapping.ContainsKey(address))
                    {
                        // If the value is not found, add the address to the not found addresses list.
                        notFoundAddresses.Add(address);
                    }
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

                // Request the IP to AS mappings.
                List<List<MercuryIpToAsMapping>> results = MercuryService.GetIpToAsMappings(notFoundAddressesList, index, count);

                // Create the AS information list.
                List<ASInformation> list = new List<ASInformation>();

                lock (this.sync)
                {
                    foreach (List<MercuryIpToAsMapping> result in results)
                    {
                        if (0 == result.Count) continue;

                        // Clear the AS information list.
                        list.Clear();

                        // For each mapping in the result.
                        foreach (MercuryIpToAsMapping mapping in result)
                        {
                            // Create an AS information.
                            ASInformation info = new ASInformation(mapping);
                            // Add the information to the list.
                            list.Add(info);
                            // Add the AS to the cache.
                            this.asMapping[mapping.AsNumber] = info;
                        }

                        // Add the address to the cache.
                        this.ipMapping.Add(result[0].Address, list);
                    }
                }
            }
        }

		#endregion
	}
}
