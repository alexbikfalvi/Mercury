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
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using DotNetApi;
using Mercury.Api;

namespace Mercury.Services
{



    /// <summary>
    /// A static class with methods for the Mercury API.
    /// </summary>
    public static class MercuryService
    {
        public const int maxRetries = 4;
        public const int maximumAddressesPerRequest = 1000;

        /// <summary>
        /// Gets the local information.
        /// </summary>
        /// <returns>An object with the local information.</returns>
        public static MercuryLocalInformation GetLocalInformation()
        {
            using (WebClient client = new WebClient())
            {
                return JsonConvert.DeserializeObject<MercuryLocalInformation>(
                    client.DownloadString("http://mercury.upf.edu/mercury/api/services/myInfo"));
            }
        }

        /// <summary>
        /// Gets the list of AS mappings corresponding to the specified list of IP addresses.
        /// </summary>
        /// <param name="address">The IP addresses.</param>
        /// <returns>The list of AS mappings.</returns>
        public static List<MercuryIpToAsMapping> GetIpToAsMappings(IPAddress address)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                return JsonConvert.DeserializeObject<List<List<MercuryIpToAsMapping>>>(
                    client.UploadString("http://mercury.upf.edu/mercury/api/services/getIp2AsnMappingsByIpsPOST",
                    string.Format("ips={0}", address)))[0];
            }
        }

        /// <summary>
        /// Gets the list of AS mappings corresponding to the specified list of IP addresses.
        /// </summary>
        /// <param name="addresses">The list of IP addresses.</param>
        /// <returns>The list of AS mappings.</returns>
        public static List<List<MercuryIpToAsMapping>> GetIpToAsMappings(IEnumerable<IPAddress> addresses)
        {
            using (WebClient client = new WebClient())
            {
                StringBuilder builder = new StringBuilder("ips=");
                foreach (IPAddress address in addresses)
                {
                    builder.AppendFormat("{0},", address);
                }
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                return JsonConvert.DeserializeObject<List<List<MercuryIpToAsMapping>>>(
                    client.UploadString("http://mercury.upf.edu/mercury/api/services/getIp2AsnMappingsByIpsPOST", builder.ToString()));
            }
        }

        /// <summary>
        /// Gets the list of AS mappings corresponding to the specified list of IP addresses.
        /// </summary>
        /// <param name="addresses">The collection of IP addresses.</param>
        /// <
        /// <returns>The list of AS mappings.</returns>
        public static List<List<MercuryIpToAsMapping>> GetIpToAsMappings(IList<IPAddress> addresses, int start, int length)
        {
            //using (WebClient client = new WebClient())
            //{
            //    StringBuilder builder = new StringBuilder("ips=");
            //    for (int index = start; index < start + length; index++)
            //    {
            //        builder.AppendFormat("{0},", addresses[index]);
            //    }
            //    client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            //    return JsonConvert.DeserializeObject<List<List<MercuryIpToAsMapping>>>(
            //        client.UploadString("http://mercury.upf.edu/mercury/api/services/getIp2AsnMappingsByIpsPOST", builder.ToString()));
            //}

            List<List<MercuryIpToAsMapping>> result = null;
            bool success = false;
            int attempt = 0;
            Exception ex = null;
            while ((attempt < maxRetries) && (!success))
            {
                attempt++;
                try{
                   using (WebClient client = new WebClient())
                   {
                        StringBuilder builder = new StringBuilder("ips=");
                        for (int index = start; index < start + length; index++)
                        {
                            builder.AppendFormat("{0},", addresses[index]);
                        }
                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        result = JsonConvert.DeserializeObject<List<List<MercuryIpToAsMapping>>>(
                            client.UploadString("http://mercury.upf.edu/mercury/api/services/getIp2AsnMappingsByIpsPOST", builder.ToString()));
                        success = true;
                   }

                } catch(Exception exception){
                    ex = exception;
                }
            }
            if (!success) throw new Exception("Problem(s) connecting with the Mercury server", ex);

            return result;
        }

        /// <summary>
        /// Gets the list of geoinformation mappings corresponding to the specified list of IP addresses.
        /// </summary>
        /// <param name="addresses">The list of IP addresses.</param>
        /// <returns>The list of geo mappings.</returns>
        public static List<MercuryIpToGeoMapping> GetIp2GeoMappings(IEnumerable<IPAddress> addresses)
        {
            using (WebClient client = new WebClient())
            {
                StringBuilder builder = new StringBuilder("ips=");
                foreach (IPAddress address in addresses)
                {
                    builder.AppendFormat("{0},", address);
                }
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                return JsonConvert.DeserializeObject<List<MercuryIpToGeoMapping>>(
                    client.UploadString("http://mercury.upf.edu/mercury/api/services/getIps2GeoPOST", builder.ToString()));
            }
        }

        /// <summary>
        /// Gets the relationship between the specified AS pair.
        /// </summary>
        /// <param name="asFirst">The first AS.</param>
        /// <param name="asSecond">The second AS></param>
        /// <returns>The relationship object.</returns>
        public static MercuryAsTracerouteRelationship GetAsRelationship(int asFirst, int asSecond)
        {
            using (WebClient client = new WebClient())
            {
                return JsonConvert.DeserializeObject<MercuryAsTracerouteRelationship>(
                    client.DownloadString(string.Format("http://mercury.upf.edu/mercury/api/services/getASRelationship/{0}/{1}", asFirst, asSecond)));
            }
        }

        /// <summary>
        /// Gets the relationships between the specified list of AS pairs.
        /// </summary>
        /// <param name="pairs">The list of AS pairs.</param>
        /// <returns>A list of relationship objects.</returns>
        public static List<MercuryAsTracerouteRelationship> GetAsRelationships(IEnumerable<Tuple<int, int>> pairs)
        {
            using (WebClient client = new WebClient())
            {
                StringBuilder builder = new StringBuilder("pairs=");
                foreach (Tuple<int, int> pair in pairs)
                {
                    builder.AppendFormat("{0}-{1},", pair.Item1, pair.Item2);
                }
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                return JsonConvert.DeserializeObject<List<MercuryAsTracerouteRelationship>>(
                    client.UploadString("http://mercury.upf.edu/mercury/api/services/getASRelationshipsPOST", builder.ToString()));
            }
        }

        /// <summary>
        /// Gets the AS-level traceroutes by destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <returns>The list of AS-level traceroutes.</returns>
        public static List<MercuryAsTraceroute> GetAsTraceroute(string destination)
        {
            using (WebClient client = new WebClient())
            {
                var json = client.DownloadString("http://mercury.upf.edu/mercury/api/services/getTracerouteASesByDst/{0}".FormatWith(destination));
                return JsonConvert.DeserializeObject<List<MercuryAsTraceroute>>(json);
            }
        }

        /// <summary>
        /// Uploads an IP-level traceroute to the Mercury service.
        /// </summary>
        /// <param name="traceroute">The traceroute.</param>
        /// <returns>The upload result.</returns>
        public static String UploadIpTraceroute(TracerouteIp traceroute)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                return client.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteIpPOST",
                    JsonConvert.SerializeObject(traceroute));
            }
        }

        /// <summary>
        /// Uploads an AS-level traceroute to the Mercury service.
        /// </summary>
        /// <param name="traceroute">The traceroute.</param>
        /// <returns>The upload result.</returns>
        public static string UploadAsTraceroute(MercuryAsTraceroute traceroute)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                return client.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteASPOST",
                    JsonConvert.SerializeObject(traceroute));
            }
        }

        /// <summary>
        /// Uploads an AS-level list of traceroutes to the Mercury service.
        /// </summary>
        /// <param name="traceroutes">The list of traceoutes.</param>
        /// <returns>The upload result.</returns>
        public static string UploadAsTraceroute(List<MercuryAsTraceroute> traceroutes)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                return client.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteASesPOST",
                    JsonConvert.SerializeObject(traceroutes));
            }
        }

        /// <summary>
        /// Uploads the traceroute settings to the Mercury service.
        /// </summary>
        /// <param name="tracerouteSettings">The traceroute settings.</param>
        /// <returns>The upload result.</returns>
        public static String UploadTracerouteSettings(MercuryTracerouteSettings tracerouteSettings)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                return client.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteSettingsPOST",
                    JsonConvert.SerializeObject(tracerouteSettings));
            }
        }
    }
}
