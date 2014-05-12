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
using System.Net;
using System.Text;
using Mercury.Api;
using Newtonsoft.Json;

namespace Mercury.Services
{
    /// <summary>
    /// A class for the Mecury service.
    /// </summary>
    public static class MercuryService
    {
        #region Static methods

        /// <summary>
        /// Gets the local information.
        /// </summary>
        /// <returns>The local information.</returns>
        public static MercuryLocalInformation GetLocalInformation()
        {
            using (WebClient webClient = new WebClient())
            {
                return JsonConvert.DeserializeObject<MercuryLocalInformation>(webClient.DownloadString("http://mercury.upf.edu/mercury/api/services/myInfo"));
            }
        }

        /// <summary>
        /// Gets the relationship information for the specifie pair of AS numbners.
        /// </summary>
        /// <param name="asFirst">The first AS number.</param>
        /// <param name="asSecond">The second AS number.</param>
        /// <returns>The relationship information.</returns>
        public static MercuryAsRelationship GetAsRelationship(int asFirst, int asSecond)
        {
            using (WebClient webClient = new WebClient())
            {
                return JsonConvert.DeserializeObject<MercuryAsRelationship>(webClient.DownloadString(string.Format("http://mercury.upf.edu/mercury/api/services/getASRelationship/{0}/{1}", asFirst, asSecond)));
            }
        }

        /// <summary>
        /// Gets the list of relationships for the specified list of pair of AS numbers.
        /// </summary>
        /// <param name="asPairs">An enumerable with pairs of AS numbers.</param>
        /// <returns>The list of relationships.</returns>
        public static List<MercuryAsRelationship> GetAsRelationships(IEnumerable<Tuple<int, int>> asPairs)
        {
            using (WebClient wc = new WebClient())
            {
                StringBuilder stringBuilder = new StringBuilder("pairs=");
                foreach (Tuple<int, int> pair in asPairs)
                {
                    stringBuilder.AppendFormat("{0}-{1},", pair.Item1, pair.Item2);
                }
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                return JsonConvert.DeserializeObject<List<MercuryAsRelationship>>(wc.UploadString("http://mercury.upf.edu/mercury/api/services/getASRelationshipsPOST", stringBuilder.ToString()));
            }
        }

        /// <summary>
        /// Gets the list of AS information corresponding to a list of IP addresses.
        /// </summary>
        /// <param name="addresses">An enumerable with the IP addresses.</param>
        /// <returns>The list of AS information.</returns>
        public static List<List<MercuryIpToAsMapping>> GetIpToAsMappings(IEnumerable<IPAddress> addresses)
        {
            using (WebClient webClient = new WebClient())
            {
                StringBuilder stringBuilder = new StringBuilder("ips=");
                foreach (IPAddress address in addresses)
                {
                    stringBuilder.AppendFormat("{0},", address.ToString());
                }
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                return JsonConvert.DeserializeObject<List<List<MercuryIpToAsMapping>>>(webClient.UploadString("http://mercury.upf.edu/mercury/api/services/getIp2AsnMappingsByIpsPOST", stringBuilder.ToString()));
            }
        }

        // TO DO
        public static List<MercuryIpToGeoMapping> GetIpToGeoMappings(string myParameters)
        {
            using (WebClient wc = new WebClient())
            {
                //string myParameters = "ips=193.145.48.3,8.8.8.85";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var json = wc.UploadString("http://mercury.upf.edu/mercury/api/services/getIps2GeoPOST", myParameters);
                //Console.WriteLine("**************\n"+json+"\n*********************");
                var ip2GeoMappings = JsonConvert.DeserializeObject<List<MercuryIpToGeoMapping>>(json);

                return ip2GeoMappings;
            }
        }

        // TO DO
        public static List<MercuryAsTraceroute> GetTracerouteASesByDst(String dst)
        {
            using (WebClient wc = new WebClient())
            {
                //String dst = "yimg.com";
                var json = wc.DownloadString("http://mercury.upf.edu/mercury/api/services/getTracerouteASesByDst/" + dst);
                //Console.WriteLine("**************\n"+json+"\n*********************");
                var tracerouteASes = JsonConvert.DeserializeObject<List<MercuryAsTraceroute>>(json);

                return tracerouteASes;
            }
        }

        public static MercuryAsTraceroute generateTracerouteAS()
        {
            //First we create 3 AS hops. Notice that we hace two HOPs number 1. 
            //     This is because we use the inferrence algorithm "true" to select the best hop
            MercuryAsTracerouteHop hop0 = new MercuryAsTracerouteHop(0, 3352, "Telefonica de Espana", "", MercuryAsTracerouteHop.HopType.As, false);
            MercuryAsTracerouteHop hop1 = new MercuryAsTracerouteHop(1, 12956, "Telefonica Backbone", "", MercuryAsTracerouteHop.HopType.As, true);
            MercuryAsTracerouteHop hop2 = new MercuryAsTracerouteHop(1, 3356, "Level-3", "", MercuryAsTracerouteHop.HopType.As, false);
            MercuryAsTracerouteHop hop3 = new MercuryAsTracerouteHop(2, 10310, "Yahoo-3", "", MercuryAsTracerouteHop.HopType.As, false);


            //Then we create the 2 AS Relationships
            MercuryAsRelationship trel0 = new MercuryAsRelationship(MercuryAsRelationship.RelationshipType.Customer, 3352, 12956, 0);
            MercuryAsRelationship trel1 = new MercuryAsRelationship(MercuryAsRelationship.RelationshipType.Peer, 12956, 10310, 1);

            //Now we create the TracerouteStats. Notice the flags value set to 2 because we have used the inference algorithm. 
            //  We have to decide which are the binary flags (int32bit)
            MercuryAsTracerouteStatistics tstats = new MercuryAsTracerouteStatistics(2, 0, 1, 0, 1, 0, 0, true, 2);

            //Finally we generate the TracerouteAS
            MercuryAsTraceroute tas = new MercuryAsTraceroute(3352, "Telefonica de Espana", IPAddress.Parse("192.168.1.2"),
                IPAddress.Parse("80.33.0.24"), "Barcelona", "Spain", 10310,
                "Yahoo-3", IPAddress.Parse("98.139.102.145"), "yimg.com", "Sunny Valley", "United States", tstats);

            tas.Hops.Add(hop0);
            tas.Hops.Add(hop1);
            tas.Hops.Add(hop2);
            tas.Hops.Add(hop3);

            tas.Relationships.Add(trel0);
            tas.Relationships.Add(trel1);

            return tas;

        }

        public static String addTracerouteAS(MercuryAsTraceroute tracerouteAS)
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

        public static String addTracerouteASes(List<MercuryAsTraceroute> tases)
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
            Guid id = Guid.NewGuid();
            Console.WriteLine("UUID generated: " + id);
            MercuryTracerouteSettings ts = new MercuryTracerouteSettings(
                id, 3, 3, 0, 32, 500, 1000, 35000, 36000, 20);
            return ts;

        }

        /// <summary>
        /// Adds a traceroute settings to the Mercury service.
        /// </summary>
        /// <param name="tracerouteSettings">The traceroute settings.</param>
        /// <returns>The settings identifier.</returns>
        public static Guid AddTracerouteSettings(MercuryTracerouteSettings tracerouteSettings)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                return Guid.Parse(webClient.UploadString(
                    "http://mercury.upf.edu/mercury/api/services/addTracerouteSettingsPOST",
                    JsonConvert.SerializeObject(tracerouteSettings)));
            }
        }


        public static MercuryIpTraceroute generateTracerouteIp()
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
            MercuryIpFlow tflow = new MercuryIpFlow(Guid.NewGuid(), MercuryIpFlow.AlgorithmType.Icmp);
            tflow.Attempts.Add(tatt);

            //We create 1 traceroute
            MercuryIpTraceroute tip = new MercuryIpTraceroute(
                IPAddress.Parse("192.168.1.2"),
                IPAddress.Parse("80.33.0.24"),
                IPAddress.Parse("98.139.102.145"),
                "yimg.com", MercuryService.AddTracerouteSettings(generateTracerouteSettings()));
            tip.Flows.Add(tflow);

            return tip;
        }


        public static String addTracerouteIp(MercuryIpTraceroute tracerouteIp)
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

        #endregion
    }
}
