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
using System.Net;
using System.Threading;
using InetApi.Net.Core;
using Mercury.Api;
using Mercury.Services;
using System.Collections.Generic;

namespace Mercury.Topology
{
	/// <summary>
	/// A delegate used for AS traceroute callback methods.
	/// </summary>
	/// <param name="result">The AS traceroute result.</param>
	public delegate void ASTracerouteCallback(ASTracerouteResult result, ASTracerouteState state);

	/// <summary>
	/// A class for an autonomous system traceroute.
	/// </summary>
	public sealed class ASTraceroute
	{
		private readonly ASTracerouteSettings settings;
		private readonly ASTracerouteCache cache;

		/// <summary>
		/// Creates a new AS traceroute instance.
		/// </summary>
		/// <param name="settings">The traceroute settings.</param>
		public ASTraceroute(ASTracerouteSettings settings)
		{
			this.settings = settings;
            this.cache = new ASTracerouteCache(this.settings);
		}

		/// <summary>
		/// Runs an AS-level traceroute starting from the specified IP-level multipath traceroute.
		/// </summary>
		/// <param name="traceroute">The IP-level multipath traceroute.</param>
		/// <param name="cancel">The cancellation token.</param>
		/// <param name="callback">The callback methods.</param>
		/// <returns>The AS-level traceroute.</returns>
		public ASTracerouteResult Run(MultipathTracerouteResult traceroute, CancellationToken cancel, ASTracerouteCallback callback)
		{
            // Create a list of IP addresses.
            HashSet<IPAddress> addresses = new HashSet<IPAddress>();

            

            // Add the addresses from the ICMP and UDP data.
            for (byte flow = 0; flow < traceroute.Settings.FlowCount; flow++)
            {
                for (byte ttl = 0; ttl < traceroute.Settings.MaximumHops - traceroute.Settings.MinimumHops + 1; ttl++)
                {
                    for (byte attempt = 0; attempt < traceroute.Settings.AttemptsPerFlow; attempt++)
                    {
                        if (traceroute.IcmpData[flow, ttl, attempt].Type != MultipathTracerouteData.ResponseType.Unknown)
                        {
                            addresses.Add(traceroute.IcmpData[flow, ttl, attempt].Address);
                        }
                        if (traceroute.UdpData[flow, ttl, attempt].Type != MultipathTracerouteData.ResponseType.Unknown)
                        {
                            addresses.Add(traceroute.UdpData[flow, ttl, attempt].Address);
                        }
                    }
                }
            }

            

            // Solve the list of IP addresses to AS information and we stored it in cache.
            List<List<MercuryIpToAsMapping>> mappings = this.cache.Get(addresses);

            // We create a multidemensional array with the AS mappings inside the "paths" variable
            ASPreviewHops[,,] paths = new ASPreviewHops[traceroute.Settings.FlowCount, traceroute.Settings.AttemptsPerFlow, 2];
            for (byte flow = 0; flow < traceroute.Settings.FlowCount; flow++)
            {
                for (byte attempt = 0; attempt < traceroute.Settings.AttemptsPerFlow; attempt++)
                {
                    paths[flow, attempt, 0] = new ASPreviewHops();
                    for (byte ttl = 0; ttl < traceroute.IcmpStatistics[flow, attempt].MaximumTimeToLive; ttl++)
                    {
                        if (traceroute.IcmpData[flow, ttl, attempt].Type != MultipathTracerouteData.ResponseType.Unknown)
                        {
                            List<MercuryIpToAsMapping> maps = this.cache.Get(traceroute.IcmpData[flow, ttl, attempt].Address);
                            
                            List<TracerouteASHop> hops = new List<TracerouteASHop>();
                            foreach(MercuryIpToAsMapping map in maps){
                                int hopTTL = ttl;
                                hops.Add( new TracerouteASHop(hopTTL, map.AsNumber, map.AsName, map.IxpName, (TracerouteASHop.Type)map.type, false) );
                            }
                            paths[flow, attempt, 0].addHops(hops);
                        }
                    }

                    paths[flow, attempt, 1] = new ASPreviewHops();
                    for (byte ttl = 0; ttl < traceroute.UdpStatistics[flow, attempt].MaximumTimeToLive; ttl++)
                    {
                        if (traceroute.UdpData[flow, ttl, attempt].Type != MultipathTracerouteData.ResponseType.Unknown)
                        {
                            List<MercuryIpToAsMapping> maps = this.cache.Get(traceroute.UdpData[flow, ttl, attempt].Address);
                            
                            List<TracerouteASHop> hops = new List<TracerouteASHop>();
                            foreach (MercuryIpToAsMapping map in maps)
                            {
                                int hopTTL = ttl;
                                hops.Add(new TracerouteASHop(hopTTL, map.AsNumber, map.AsName, map.IxpName, (TracerouteASHop.Type)map.type, false));
                            }
                            paths[flow, attempt, 1].addHops(hops);
                        }
                    }

                }
            }


            /*
             * AQUIIIIIIIIIIIIIIII
             * 
             */ 
            // ALEX I AM HERE
            //Now we check missing and multiple hops and we try to correct it
            int flows = paths.GetLength(0);
            int attemptsPerFlow = paths.GetLength(1);
            int algorithms = paths.GetLength(2);
            ASPreviewHops[, ,] pathsAggrAS1 = new ASPreviewHops[flows, attemptsPerFlow, algorithms];
            ASPreviewHops[, ,] pathsAggrAS2 = new ASPreviewHops[flows, attemptsPerFlow, algorithms];
            ASPreviewHops[, ,] pathsAggrAS3 = new ASPreviewHops[flows, attemptsPerFlow, algorithms];
            //ASPreviewHops[, ,] pathsAggrAS4 = new ASPreviewHops[flows, attemptsPerFlow, algorithms];
            //ASPreviewHops[, ,] pathsAggrAS5 = new ASPreviewHops[flows, attemptsPerFlow, algorithms];
            for (int flow = 0; flow < flows; flow++)
            {
                for (int attempt = 0; attempt < attemptsPerFlow; attempt++)
                {
                    for (int alg = 0; alg < algorithms; alg++)
                    {
                        if (alg == 0) //ICMP
                        {
                            //processASPreviewHops(paths[flow, attempt, alg]);

                            pathsAggrAS1[flow, attempt, alg] = aggregateMissings(paths[flow, attempt, alg]);
                            pathsAggrAS2[flow, attempt, alg] = removeMissingInMiddleSameAS(pathsAggrAS1[flow, attempt, alg]);
                            pathsAggrAS3[flow, attempt, alg] = aggregateSameASes(pathsAggrAS2[flow, attempt, alg]);
                            //pathsAggrAS4[flow, attempt, alg] = removeLoops(pathsAggrAS3[flow, attempt, alg]);
                            //pathsAggrAS5[flow, attempt, alg] = aggregateSameASes(pathsAggrAS4[flow, attempt, alg]);

                        }
                        if (alg == 1) //UDP
                        {
                            //processASPreviewHops(paths[flow, attempt, alg]);
                        }
            
                    }
                }
            }

                /*

                //LocalInformation localInformation = MercuryService.GetLocalInformation();
                MercuryLocalInformation myInfo = MercuryService.GetLocalInformation();

                List<MercuryIpToAsMapping> map = MercuryService.GetIpToAsMappings(IPAddress.Parse("8.8.8.85"));

                //An example for obtaining the IP2ASN mappings in BULK max 1000 ips
                List<List<MercuryIpToAsMapping>> ip2asMappings = MercuryService.GetIpToAsMappings(new IPAddress[]
                    {
                        IPAddress.Parse("193.145.48.3"), IPAddress.Parse("8.8.8.85")
                    });

                //An example for obtaining the IP2GEO in BULK mode max 1000 ips
                List<MercuryIpToGeoMapping> ip2geoMappings = MercuryService.GetIp2GeoMappings(new IPAddress[]
                    {
                        IPAddress.Parse("193.145.48.3"), IPAddress.Parse("8.8.8.85")
                    });

                //An example for obtaining the AS rels in BULK mode
                List<MercuryAsTracerouteRelationship> asRels = MercuryService.GetAsRelationships(new Tuple<int, int>[]
                    {
                        new Tuple<int, int>(2, 3),
                        new Tuple<int, int>(766, 3356),
                        new Tuple<int, int>(2589, 6985)
                    });

                //An example for obtaining the AS rel betwenn AS2 and AS3
                MercuryAsTracerouteRelationship asRel = MercuryService.GetAsRelationship(2, 3);

                //Alex you don't need this... but just in case...
                //An example for obtaining TracerouteASes by destination domain
                String dst = "yimg.com";
                List<TracerouteAS> tases = MercuryService.GetTracerouteASesByDst(dst);

                //An example for uploading a TracerouteAS
                //First we generate a dummy object. Then this must be obtained from the result of the algorithm
                TracerouteAS tas = MercuryService.generateTracerouteAS();
                String result1 = MercuryService.addTracerouteAS(tas);
                //An example for uploading many TracerouteASes in BULK mode
                //First we generate dummy objects. Then this must be obtained from the result of the processing algorithm
                List<TracerouteAS> tases2 = new List<TracerouteAS>();
                tases2.Add(tas);
                tases2.Add(tas);
                tases2.Add(tas);
                String result2 = MercuryService.addTracerouteASes(tases2);

                //An example for uploading TracerouteSettings. 
                //It returns the same ID if is new, it returns the existing ID if it already exists
                Mercury.Api.MercuryTracerouteSettings tset = MercuryService.generateTracerouteSettings();
                String result3 = MercuryService.addTracerouteSettings(tset);

                //An example for uploading a TracerouteIp. Then this must be obtained from the result of the alogrithm
                TracerouteIp tip = MercuryService.generateTracerouteIp();
                String result4 = MercuryService.addTracerouteIp(tip);
                */


              return null;
		}



        /*
         *AQUIIIIIIIIIIIIIIIIIIIII 
         * 
         */
        //I am here, preparing the algorithm

        private ASPreviewHops aggregateMissings(ASPreviewHops asPreviewHops)
        {

            ASPreviewHops asPreviewHopsAux = new ASPreviewHops();
            bool prevHopMissing = false;
            for (int i = 0; i < asPreviewHops.hops.Count; i++ )
            {
                    if (asPreviewHops.hops[i].Count != 0) //If not missing we add it
                    {
                        if (prevHopMissing == false)
                        {
                            asPreviewHopsAux.addHops(asPreviewHops.hops[i]);
                        }
                        else
                        {
                            asPreviewHopsAux.addHops(asPreviewHops.hops[i-1]); //We add the previous missing. ONly 1
                            asPreviewHopsAux.addHops(asPreviewHops.hops[i]); //We add the hops
                        }
                        prevHopMissing = false;
                    }
                    else //If missing
                    {
                        prevHopMissing = true;
                    }
             }

            return asPreviewHopsAux;
        }

        private ASPreviewHops removeMissingInMiddleSameAS(ASPreviewHops asPreviewHops)
        {
            ASPreviewHops asPreviewHopsAux = new ASPreviewHops();

            //First we add the first and/or last hop if they are missings
            if (asPreviewHops.hops[0].Count == 0) //missing at position 0 
            {
                asPreviewHopsAux.addHops(asPreviewHops.hops[0]);
            }
            if (asPreviewHops.hops[asPreviewHops.hops.Count-1].Count == 0) //missing at last hop
            {
                asPreviewHopsAux.addHops(asPreviewHops.hops[0]);
            }

            if (asPreviewHops.hops.Count > 2) //We need at least 3 hops
            {

                for (int i = 1; i < asPreviewHops.hops.Count - 1; i++) //We do not need to start from position 0 and we need to end at Count-1
                {
                    if (asPreviewHops.hops[i].Count != 0) //If not missing we add it
                    {
                        asPreviewHopsAux.addHops(asPreviewHops.hops[i]); //We add the hops
                    }
                    else //If missing
                    {
                        //We only add the missing hop when it is not in the middle of same AS
                        if (asPreviewHops.hops[i - 1][0].asNumber != asPreviewHops.hops[i + 1][0].asNumber)
                        {
                            asPreviewHopsAux.addHops(asPreviewHops.hops[i]); //We add the missing hops
                        }

                    }
                }
            }
            return asPreviewHopsAux;
        }

        private ASPreviewHops aggregateSameASes(ASPreviewHops asPreviewHops)
        {

            ASPreviewHops asPreviewHopsAux = new ASPreviewHops();
            bool firstHop = true;
            bool prevHopMissing = false;
            bool prevHopSame = false;
            bool prevHopMulti = false;
            for (int i = 0; i < asPreviewHops.hops.Count; i++)
            {

                if (firstHop) //If first hop...
                {
                    if (asPreviewHops.hops[i].Count != 0) //If not missing...
                    {
                        if (asPreviewHops.hops[i].Count > 1) //If multi...
                        {
                            prevHopMissing = false;
                            prevHopSame = false;
                            prevHopMulti = true;
                            asPreviewHopsAux.addHops(asPreviewHops.hops[i]); //We save the multi
                        }
                        else //If not multi...
                        {
                            prevHopMissing = false;
                            prevHopSame = false;
                            prevHopMulti = false;
                        }
                    }
                    else //If missing
                    {
                        prevHopMissing = true;
                        prevHopSame = false;
                        prevHopMulti = false;
                        asPreviewHopsAux.addHops(asPreviewHops.hops[i]); //We save the missing
                    }
                    firstHop = false;
                }
                
                
                
                else //If more than second hop...
                {
                    if (asPreviewHops.hops[i].Count != 0) //If not missing...
                    {
                        if (asPreviewHops.hops[i].Count > 1) //If multi...
                        {
                            if (prevHopMissing == prevHopSame == prevHopMulti == false) //If previous was an AS hop
                            {
                                asPreviewHopsAux.addHops(asPreviewHops.hops[i - 1]);
                            }
                            prevHopMissing = false;
                            prevHopSame = false;
                            prevHopMulti = true;
                            asPreviewHopsAux.addHops(asPreviewHops.hops[i]); //We save the multi
                        }
                        else //If not multi...
                        {
                            if ( !prevHopMissing && !prevHopMulti ) //If previous hop is NOT missing and NOT multi
                            {
                                if (asPreviewHops.hops[i][0].asNumber == asPreviewHops.hops[i - 1][0].asNumber) //if same as previous
                                {
                                    prevHopMissing = false;
                                    prevHopSame = true;
                                    prevHopMulti = false;
                                }
                                else
                                {
                                    //if (prevHopSame) //If previous was same but now is different, we add the previous same, only 1
                                    //{
                                        asPreviewHopsAux.addHops(asPreviewHops.hops[i - 1]);
                                    //}
                                    prevHopMissing = false;
                                    prevHopSame = false;
                                    prevHopMulti = false;
                                }
                                //We check if last Hop and we add it
                                if (i == asPreviewHops.hops.Count-1)
                                {
                                    asPreviewHopsAux.addHops(asPreviewHops.hops[i]);
                                }
                            }
                            else //If previous is missing or multi
                            {
                                prevHopMissing = false;
                                prevHopSame = false;
                                prevHopMulti = false;
                            }

                        }
                    }
                    else //If missing
                    {
                        if (prevHopMissing == prevHopSame == prevHopMulti == false) //If previous was an AS hop
                        {
                            asPreviewHopsAux.addHops(asPreviewHops.hops[i - 1]);
                        }
                        prevHopMissing = true;
                        prevHopSame = false;
                        prevHopMulti = false;
                        asPreviewHopsAux.addHops(asPreviewHops.hops[i]); //We save the missing
                    }
                }

            }

            return asPreviewHopsAux;
        }

        //private ASPreviewHops removeLoops(ASPreviewHops asPreviewHops)
        //{
        //    ASPreviewHops asPreviewHopsAux = new ASPreviewHops();

        //    //First we add the first and/or last hop if they are missings
        //    if (asPreviewHops.hops[0].Count == 0) //missing at position 0 
        //    {
        //        asPreviewHopsAux.addHops(asPreviewHops.hops[0]);
        //    }
        //    if (asPreviewHops.hops[asPreviewHops.hops.Count - 1].Count == 0) //missing at last hop
        //    {
        //        asPreviewHopsAux.addHops(asPreviewHops.hops[0]);
        //    }

        //    if (asPreviewHops.hops.Count > 2) //We need at least 3 hops
        //    {

        //        for (int i = 1; i < asPreviewHops.hops.Count - 1; i++) //We do not need to start from position 0 and we need to end at Count-1
        //        {
        //            if (asPreviewHops.hops[i].Count != 0) //If not missing we add it
        //            {
        //                //First we check if its in the middle of some missing
        //                if (asPreviewHops.hops[i - 1].Count != asPreviewHops.hops[i + 1][0].asNumber)

        //                //We only add the AS hop when it is not in the middle of same AS
        //                if (asPreviewHops.hops[i - 1][0].asNumber != asPreviewHops.hops[i + 1][0].asNumber)
        //                {
        //                    asPreviewHopsAux.addHops(asPreviewHops.hops[i]); //We add the missing hops
        //                }
        //            }
        //            else //If missing
        //            {
        //                asPreviewHopsAux.addHops(asPreviewHops.hops[i]); //We add the hops
        //            }
        //        }
        //    }
        //    return asPreviewHopsAux;
        //}
    
    }
}
