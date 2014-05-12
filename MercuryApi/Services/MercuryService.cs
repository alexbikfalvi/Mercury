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
using Mercury.Api;
using Newtonsoft.Json;

namespace Mercury.Services
{
    /// <summary>
    /// A static class with methods for the Mercury API.
    /// </summary>
    public static class MercuryService
    {
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
            using (WebClient client = new WebClient())
            {
                StringBuilder builder = new StringBuilder("ips=");
                for (int index = start; index < start + length; index++)
                {
                    builder.AppendFormat("{0},", addresses[index]);
                }
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                return JsonConvert.DeserializeObject<List<List<MercuryIpToAsMapping>>>(
                    client.UploadString("http://mercury.upf.edu/mercury/api/services/getIp2AsnMappingsByIpsPOST", builder.ToString()));
            }
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

        public static List<TracerouteAS> GetTracerouteASesByDst(String dst)
        {
            using (WebClient wc = new WebClient())
            {
                //String dst = "yimg.com";
                var json = wc.DownloadString("http://mercury.upf.edu/mercury/api/services/getTracerouteASesByDst/" + dst);
                //Console.WriteLine("**************\n"+json+"\n*********************");
                var tracerouteASes = JsonConvert.DeserializeObject<List<TracerouteAS>>(json);

                return tracerouteASes;
            }
        }



        public static TracerouteAS generateTracerouteAS()
        {

            //First we create 3 AS hops. Notice that we hace two HOPs number 1. 
            //     This is because we use the inferrence algorithm "true" to select the best hop
            TracerouteASHop hop0 = new TracerouteASHop(0, 3352, "Telefonica de Espana", "", TracerouteASHop.Type.AS, false);
            TracerouteASHop hop1 = new TracerouteASHop(1, 12956, "Telefonica Backbone", "", TracerouteASHop.Type.AS, true);
            TracerouteASHop hop2 = new TracerouteASHop(1, 3356, "Level-3", "", TracerouteASHop.Type.AS, false);
            TracerouteASHop hop3 = new TracerouteASHop(2, 10310, "Yahoo-3", "", TracerouteASHop.Type.AS, false);


            //Then we create the 2 AS Relationships
            MercuryAsTracerouteRelationship trel0 = new MercuryAsTracerouteRelationship(MercuryAsTracerouteRelationship.RelationshipType.SiblingToSibling, 3352, 12956, 0);
            MercuryAsTracerouteRelationship trel1 = new MercuryAsTracerouteRelationship(MercuryAsTracerouteRelationship.RelationshipType.PeerToPeer, 12956, 10310, 1);

            //Now we create the TracerouteStats. Notice the flags value set to 2 because we have used the inference algorithm. 
            //  We have to decide which are the binary flags (int32bit)
            TracerouteASStats tstats = new TracerouteASStats(2, 0, 1, 0, 1, 0, 0, true, 1);

            //Finally we generate the TracerouteAS
            TracerouteAS tas = new TracerouteAS(3352, "Telefonica de Espana", "192.168.1.2",
                "80.33.0.24", "Barcelona", "Spain", 10310,
                "Yahoo-3", "98.139.102.145", "y.com", "Sunny Valley", "United States", DateTime.UtcNow, tstats);

            tas.tracerouteASHops.Add(hop0);
            tas.tracerouteASHops.Add(hop1);
            tas.tracerouteASHops.Add(hop2);
            tas.tracerouteASHops.Add(hop3);

            tas.tracerouteASRelationships.Add(trel0);
            tas.tracerouteASRelationships.Add(trel1);

            return tas;

        }

        public static String addTracerouteAS(TracerouteAS tracerouteAS)
        {
            using (WebClient wc = new WebClient())
            {
                //We serialize
                //string myData = JsonConvert.SerializeObject(generateTracerouteAS());
                string myData = JsonConvert.SerializeObject(tracerouteAS);
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                var result = wc.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteASPOST", myData);
                //Console.WriteLine("**************\n"+result+"\n*********************");

                return result;
            }
        }

        public static String addTracerouteASes(List<TracerouteAS> tases)
        {
            using (WebClient wc = new WebClient())
            {
                //We add an array with 3 TracerouteAS(es)
                //List<TracerouteAS> tases = new List<TracerouteAS>();
                //tases.Add(generateTracerouteAS());
                //tases.Add(generateTracerouteAS());
                //tases.Add(generateTracerouteAS());

                string myData = JsonConvert.SerializeObject(tases);
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                var result = wc.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteASesPOST", myData);
                //Console.WriteLine("**************\n"+result+"\n*********************");

                return result;
            }
        }


        public static MercuryTracerouteSettings generateTracerouteSettings()
        {
            String UUID = Guid.NewGuid().ToString();
            Console.WriteLine("UUID generated: " + UUID);
            MercuryTracerouteSettings ts = new MercuryTracerouteSettings(
                UUID, 3, 3, 0, 32, 500, 1000, 35000, 36000, 20);
            return ts;

        }

        public static String addTracerouteSettings(MercuryTracerouteSettings tracerouteSettings)
        {
            using (WebClient wc = new WebClient())
            {

                //string myData = JsonConvert.SerializeObject(generateTracerouteSettings());
                string myData = JsonConvert.SerializeObject(tracerouteSettings);
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                var result = wc.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteSettingsPOST", myData);
                //Console.WriteLine("**************\n"+result+"\n*********************");

                return result;
            }
        }


        public static TracerouteIp generateTracerouteIp()
        {
            //First we create 4 Ip hops
            TracerouteIpHop thop0 = new TracerouteIpHop(TracerouteIpHop.State.REQ_SENT, TracerouteIpHop.Type.ECHO_REPLY, "192.168.1.1", 1);
            TracerouteIpHop thop1 = new TracerouteIpHop(TracerouteIpHop.State.REQ_SENT, TracerouteIpHop.Type.ECHO_REPLY, "194.224.58.10", 2);
            TracerouteIpHop thop2 = new TracerouteIpHop(TracerouteIpHop.State.REQ_SENT, TracerouteIpHop.Type.ECHO_REPLY, "84.16.0.1", 3);
            TracerouteIpHop thop3 = new TracerouteIpHop(TracerouteIpHop.State.REQ_SENT, TracerouteIpHop.Type.ECHO_REPLY, "98.139.102.145", 4);

            //We create 1 attempt
            TracerouteIpAttempt tatt = new TracerouteIpAttempt(TracerouteIpAttempt.State.COMPLETED, 4, Guid.NewGuid().ToString());

            tatt.tracerouteIpHops.Add(thop0);
            tatt.tracerouteIpHops.Add(thop1);
            tatt.tracerouteIpHops.Add(thop2);
            tatt.tracerouteIpHops.Add(thop3);

            //We create 1 flow
            TracerouteIpFlow tflow = new TracerouteIpFlow(TracerouteIpFlow.Algorithm.ICMP, Guid.NewGuid().ToString());
            tflow.tracerouteIpAttemps.Add(tatt);

            //We create 1 traceroute
            TracerouteIp tip = new TracerouteIp("192.168.1.2", "80.33.0.24", "98.139.102.145", "yimg.com", addTracerouteSettings(generateTracerouteSettings()), DateTime.UtcNow );
            tip.tracerouteIpFlows.Add(tflow);

            return tip;
        }


        public static String addTracerouteIp(TracerouteIp tracerouteIp)
        {
            using (WebClient wc = new WebClient())
            {
                //string myData = JsonConvert.SerializeObject(generateTracerouteIp());
                string myData = JsonConvert.SerializeObject(tracerouteIp);
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                var result = wc.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteIpPOST", myData);
                //Console.WriteLine("**************\n"+result+"\n*********************");

                return result;
            }
        }


        /*Not yet implemented in Mercury Server*/
        //public static String addTracerouteIps()
        //{
        //    using (WebClient wc = new WebClient())
        //    {
        //        List<TracerouteIp> tips = new List<TracerouteIp>();
        //        tips.Add(generateTracerouteIp());
        //        tips.Add(generateTracerouteIp());
        //        tips.Add(generateTracerouteIp());

        //        string myData = JsonConvert.SerializeObject(tips);
        //        wc.Headers[HttpRequestHeader.ContentType] = "application/json";
        //        var result = wc.UploadString("http://mercury.upf.edu/mercury/api/services/addTracerouteIpPOST", myData);
        //        //Console.WriteLine("**************\n"+result+"\n*********************");

        //        return result;
        //    }
        //}

    }
}
