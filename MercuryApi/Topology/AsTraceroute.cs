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
using System.Linq;
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
            // Create the list of algorithms.
            List<byte> algorithms = new List<byte>();
            if ((traceroute.Settings.Algorithm & MultipathTraceroute.MultipathAlgorithm.Icmp) != 0) algorithms.Add(0);
            if ((traceroute.Settings.Algorithm & MultipathTraceroute.MultipathAlgorithm.Udp) != 0) algorithms.Add(1);

            // Create a list of IP addresses.
            HashSet<IPAddress> addresses = new HashSet<IPAddress>();

            // Add the addresses from the ICMP and UDP data.
            foreach (MultipathTracerouteResult.ResultAlgorithm algorithm in traceroute.Algorithms)
            {
                for (byte flow = 0; flow < traceroute.Settings.FlowCount; flow++)
                {
                    for (byte attempt = 0; attempt < traceroute.Settings.AttemptsPerFlow; attempt++)
                    {
                        for (byte ttl = 0; ttl < traceroute.Statistics[(byte)algorithm, flow, attempt].MaximumTimeToLive - traceroute.Settings.MinimumHops + 1; ttl++)
                        {
                            if (traceroute.Data[(byte)algorithm, flow, attempt, ttl].State == MultipathTracerouteData.DataState.ResponseReceived)
                            {
                                addresses.Add(traceroute.Data[(byte)algorithm, flow, attempt, ttl].Address);
                            }
                        }
                    }
                }
            }

            // Add the public address and remote address. 
            MercuryLocalInformation localInformation = MercuryService.GetLocalInformation();
            addresses.Add(localInformation.Address);
            addresses.Add(traceroute.RemoteAddress);

            // Create the AS traceroute result.
            ASTracerouteResult result = new ASTracerouteResult(traceroute, callback);

            // Solve the list of IP addresses to AS information and we stored it in cache.
            //List<List<MercuryIpToAsMapping>> mappings = this.cache.Get(addresses);


            /*
            //We pre-load the publicIp and the dstIp mappings
            //public src
            List<MercuryIpToAsMapping> publicMaps = this.cache.Get(localInformation.Address);
            List<MercuryAsTracerouteHop> publicHops = new List<MercuryAsTracerouteHop>();
            foreach (MercuryIpToAsMapping map in publicMaps)
            {
                publicHops.Add(new MercuryAsTracerouteHop(0, map.AsNumber, map.AsName, map.IxpName, map.Type, false));
            }
            //remote dst
            List<MercuryIpToAsMapping> remoteMaps = this.cache.Get(traceroute.RemoteAddress);
            List<MercuryAsTracerouteHop> remoteHops = new List<MercuryAsTracerouteHop>();


            //We preload the src and dst geoMappings
            String[] geoMappings = getGeoMappings(localInformation.Address, traceroute.RemoteAddress);
            */


            /*
             * STEP 1: Process path hop anomalies.
             */

            // For each algorithm.
            foreach (MultipathTracerouteResult.ResultAlgorithm algorithm in traceroute.Algorithms)
            {
                // For each flow.
                for (byte flow = 0; flow < traceroute.Settings.FlowCount; flow++)
                {
                    // For each attempt.
                    for (byte attempt = 0; attempt < traceroute.Settings.AttemptsPerFlow; attempt++)
                    {
                        // Create a new path.
                        result.PathsStep1[(byte)algorithm, flow, attempt] = new ASTraceroutePath();
                        // For each TTL.
                        for (byte ttl = 0; ttl < traceroute.Statistics[(byte)algorithm, flow, attempt].MaximumTimeToLive - traceroute.Settings.MinimumHops + 1; ttl++)
                        {
                            // If a response was received.
                            if (traceroute.Data[(byte)algorithm, flow, attempt, ttl].State == MultipathTracerouteData.DataState.ResponseReceived)
                            {
                                // Get the list of corresponding ASes.
                                List<ASInformation> ases = this.cache.Get(traceroute.Data[(byte)algorithm, flow, attempt, ttl].Address);

                                // Add a new hop for the list of ASes.
                                result.PathsStep1[(byte)algorithm, flow, attempt].AddHop(ases);
                            }
                            else
                            {
                                // Add a new hop for a missing AS.
                                result.PathsStep1[(byte)algorithm, flow, attempt].AddHop();
                            }
                        }

                        /*
                        //Here we add the publicAddress
                        pathsStep1[flow, attempt, 0].addHopsAtBegining(publicHops); //ICMP
                        pathsStep1[flow, attempt, 1].addHopsAtBegining(publicHops); //UDP
                        //Here we add the remoteAddress
                        foreach (MercuryIpToAsMapping map in remoteMaps)
                        {
                            remoteHops.Add(new MercuryAsTracerouteHop(traceroute.UdpStatistics[flow, attempt].MaximumTimeToLive, map.AsNumber, map.AsName, map.IxpName, map.Type, false));
                        }
                        pathsStep1[flow, attempt, 0].addHopsAtEnd(remoteHops); //ICMP
                        pathsStep1[flow, attempt, 1].addHopsAtEnd(remoteHops); //UDP
                        */
                    }
                }
            }

            // Call the callback method.
            result.Callback(ASTracerouteState.StateType.Step1);

            /*
             * STEP 2: Aggregate hops for the same path.
             */

            /*
            int flows = pathsStep1.GetLength(0);
            int attemptsPerFlow = pathsStep1.GetLength(1);
            ASTraceroutePath[, ,] pathsStep2 = new ASTraceroutePath[flows, attemptsPerFlow, 2];
            //ASTraceroutePath[, ,] pathsAggrAS2 = new ASTraceroutePath[flows, attemptsPerFlow, algorithms];

            //List<MercuryAsTraceroute> tracerouteASes = new List<MercuryAsTraceroute>();
            foreach (byte algorithm in algorithms)
            {
                for (int flow = 0; flow < flows; flow++)
                {
                    for (int attempt = 0; attempt < attemptsPerFlow; attempt++)
                    {
                        pathsStep2[flow, attempt, algorithm] = aggregateHops(pathsStep1[flow, attempt, algorithm]);
                    }
                }
            }
             */

            /*
             * STEP 3: Aggregate attempt paths for the same flow.
             */

            /*
            // Create the list of flow aggregated paths.
            ASTraceroutePath[,] pathsStep3 = new ASTraceroutePath[traceroute.Settings.FlowCount, 2];
            // Compare the paths for different attempts in the same flow.
            HashSet<ASTraceroutePath> attempts = new HashSet<ASTraceroutePath>();
            foreach (byte algorithm in algorithms)
            {
                for (byte flow = 0; flow < traceroute.Settings.AttemptsPerFlow; flow++)
                {
                    // Clear the attempt.
                    attempts.Clear();
                    for (byte attempt = 0; attempt < traceroute.Settings.AttemptsPerFlow; attempt++)
                    {
                        // Add the attempt to the list of attempts.
                        attempts.Add(pathsStep2[flow, attempt, algorithm]);
                    }
                    // If there is only one path.
                    if (attempts.Count == 1)
                    {
                        // Add the path to the result.
                        pathsStep3[flow, algorithm] = attempts.First();
                    }
                    else
                    {
                        pathsStep3[flow, algorithm] = new ASTraceroutePath(ASTracerouteFlags.FlowDistinctAttempts);
                    }
                }
            }
             */

            /*
             * Step 4: Save unique flows.
             */

            /*
            // Create the list of unique flows.
            HashSet<ASTraceroutePath> pathsStep4 = new HashSet<ASTraceroutePath>();
            foreach (byte algorithm in algorithms)
            {
                for (byte flow = 0; flow < traceroute.Settings.AttemptsPerFlow; flow++)
                {
                    pathsStep4.Add(pathsStep3[flow, algorithm]);
                }
            }
             */

                //Finally we add the MercuryAsTraceroute(es) to the Mercury Platform
                //String result = MercuryService.addTracerouteASes(tracerouteASes);


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
                List<MercuryAsTraceroute> tases = MercuryService.GetTracerouteASesByDst(dst);

                //An example for uploading a MercuryAsTraceroute
                //First we generate a dummy object. Then this must be obtained from the result of the algorithm
                MercuryAsTraceroute tas = MercuryService.generateTracerouteAS();
                String result1 = MercuryService.addTracerouteAS(tas);
                //An example for uploading many TracerouteASes in BULK mode
                //First we generate dummy objects. Then this must be obtained from the result of the processing algorithm
                List<MercuryAsTraceroute> tases2 = new List<MercuryAsTraceroute>();
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

        /*
        private ASTraceroutePath aggregateHops(ASTraceroutePath asPreviewHops)
        {
            ASTraceroutePath asPreviewHopsAux = new ASTraceroutePath();
            for (int i = 1; i < asPreviewHops.hops.Count-1; i++)
            {

                if (asPreviewHopsAux.hops.Count > 0)
                {
                    if (! asPreviewHops.hops[i].isMissing(asPreviewHopsAux.hops[asPreviewHopsAux.hops.Count - 1])) //We compare with the last index of the aux list
                    {
                        if ( asPreviewHops.hops[i].getEqualUnique(asPreviewHopsAux.hops[asPreviewHopsAux.hops.Count - 1]) == null)
                        {
                            if (asPreviewHops.hops[i].getEqualUniqueToMultiple(asPreviewHopsAux.hops[asPreviewHopsAux.hops.Count - 1]) != null)
                            {
                                MercuryAsTracerouteHop h = asPreviewHops.hops[i].getEqualUniqueToMultiple(asPreviewHopsAux.hops[asPreviewHopsAux.hops.Count - 1]);

                                //asPreviewHops.hops[i].candidates.Clear(); //First we clear the list
                                //asPreviewHops.hops[i].candidates.Add( h.AsNumber, h); //then we add the hop
                                //asPreviewHops.hops.RemoveAt(i - 1); //We remove the previous hop

                                asPreviewHopsAux.hops.RemoveAt(asPreviewHopsAux.hops.Count - 1);
                                ASTracerouteHop hop = new ASTracerouteHop();
                                hop.candidates.Add(h.AsNumber, h);
                                asPreviewHopsAux.hops.Add(hop);

                            }
                            else
                            {
                                if (asPreviewHops.hops[i].getEqualMultipleToMultiple(asPreviewHopsAux.hops[asPreviewHopsAux.hops.Count - 1]) != null)
                                {
                                    MercuryAsTracerouteHop h = asPreviewHops.hops[i].getEqualMultipleToMultiple(asPreviewHopsAux.hops[asPreviewHopsAux.hops.Count - 1]);
                                    ASTracerouteHop hop = new ASTracerouteHop();
                                    hop.candidates.Add(h.AsNumber, h);
                                    asPreviewHopsAux.hops.Add(hop);
                                    
                                }
                                else //is differentes ases (AS0-AS1), different multiple ases (AS0/AS1 - AS2/AS3) or missing-AS
                                {
                                    //We check for missings in the middle of same AS
                                    int j = 0;
                                    for (j = i; j < asPreviewHops.hops.Count; j++)
                                    {
                                        if (asPreviewHops.hops[j].candidates.Count > 0)
                                        {
                                            //z = j - i; //j is position of next hop with at least 1 AS.
                                            break;
                                        }
                                    }
                                    if (asPreviewHops.hops[i].isMissingInMiddleSameAS(asPreviewHopsAux.hops[asPreviewHopsAux.hops.Count - 1], asPreviewHops.hops[j]))
                                    {
                                        i = j;
                                    }
                                    else //No missing in the middle of same AS
                                    {
                                        asPreviewHopsAux.hops.Add(asPreviewHops.hops[i]);
                                    }

                                }
                            }
                        }
                    }

                }
                else
                {
                    if (asPreviewHops.hops[i].isMissing(asPreviewHops.hops[i - 1])) 
                    {
                        //asPreviewHops.hops.RemoveAt(i-1);
                        asPreviewHopsAux.hops.Add(new ASTracerouteHop());
                    }
                    else
                    {
                        if (asPreviewHops.hops[i].getEqualUnique(asPreviewHops.hops[i - 1]) != null)
                        {
                            //asPreviewHops.hops.RemoveAt(i - 1);
                            asPreviewHopsAux.hops.Add(asPreviewHops.hops[i]);
                        }
                        else
                        {
                            if (asPreviewHops.hops[i].getEqualUniqueToMultiple(asPreviewHops.hops[i - 1]) != null)
                            {
                                MercuryAsTracerouteHop h = asPreviewHops.hops[i].getEqualUniqueToMultiple(asPreviewHops.hops[i - 1]);

                                //asPreviewHops.hops[i].candidates.Clear(); //First we clear the list
                                //asPreviewHops.hops[i].candidates.Add( h.AsNumber, h); //then we add the hop
                                //asPreviewHops.hops.RemoveAt(i - 1); //We remove the previous hop

                                ASTracerouteHop hop = new ASTracerouteHop();
                                hop.candidates.Add(h.AsNumber, h);
                                asPreviewHopsAux.hops.Add(hop);

                            }
                            else
                            {
                                if (asPreviewHops.hops[i].getEqualMultipleToMultiple(asPreviewHops.hops[i - 1]) != null)
                                {
                                    MercuryAsTracerouteHop h = asPreviewHops.hops[i].getEqualMultipleToMultiple(asPreviewHops.hops[i - 1]);
                                    ASTracerouteHop hop = new ASTracerouteHop();
                                    hop.candidates.Add(h.AsNumber, h);
                                    asPreviewHopsAux.hops.Add(hop);
                                }
                                else //is differentes ases (AS0-AS1) or missing-AS
                                {
                                    asPreviewHopsAux.hops.Add(asPreviewHops.hops[i]);
                                }
                            }
                        }
                    }
                }
                
            }

            return asPreviewHopsAux;
        }
        */



        /*
        private ASTraceroutePath obtainASRelationships(ASTraceroutePath asPreviewHops)
        {
            for (byte i = 0; i < asPreviewHops.hops.Count - 1; i++) //We end before the last hop
            {
                if (asPreviewHops.hops[i].candidates.Count != 0 && asPreviewHops.hops[i + 1].candidates.Count != 0) //If NOT missing hops
                {
                    int as0 = new List<MercuryAsTracerouteHop>(asPreviewHops.hops[i].candidates.Values)[0].AsNumber;
                    int as1 = new List<MercuryAsTracerouteHop>(asPreviewHops.hops[i+1].candidates.Values)[0].AsNumber;
                    MercuryAsTracerouteRelationship rel = MercuryService.GetAsRelationship(as0, as1);
                    rel.Hop = i;
                    //asPreviewHops.relationships[i] = rel;
                }
            }
            return asPreviewHops;
        }
        */

        /*
        private MercuryAsTracerouteStats obtainTracerouteStatistics(ASTraceroutePath asPreviewHops)
        {
            int asHops = 0;
            int c2pRels = 0,p2pRels = 0,p2cRels = 0,s2sRels = 0,ixpRels = 0,nfRels = 0;
            bool completed = false;
            int flags = 0x0; //asPreviewHops.flags.

            //We count the asHops
            asHops=asPreviewHops.hops.Count;


            //We count the asRelationships
            
            foreach (MercuryAsTracerouteRelationship rel in asPreviewHops.relationships.Values)
            {
                if (rel.Relationship == MercuryAsTracerouteRelationship.RelationshipType.CustomerToProvider) {c2pRels++;}
                else if(rel.Relationship == MercuryAsTracerouteRelationship.RelationshipType.PeerToPeer) {p2pRels++;}
                else if(rel.Relationship == MercuryAsTracerouteRelationship.RelationshipType.ProviderToCustomer) {p2cRels++;}
                else if(rel.Relationship == MercuryAsTracerouteRelationship.RelationshipType.SiblingToSibling) {s2sRels++;}
                else if (rel.Relationship == MercuryAsTracerouteRelationship.RelationshipType.InternerExchangePoint) { ixpRels++; }
                else if (rel.Relationship == MercuryAsTracerouteRelationship.RelationshipType.NotFound) { nfRels++; }
            }

            if (flags < 0x2) completed = true;

            return new MercuryAsTracerouteStats(asHops,c2pRels,p2pRels,p2cRels,s2sRels,ixpRels,nfRels,completed,flags);
        }
        

        private String[] getGeoMappings(IPAddress srcIp, IPAddress dstIp)
        {
            //Geo
            
            String srcCity = "", srcCountry = "", dstCity = "", dstCountry = "";
            List<MercuryIpToGeoMapping> ip2geoMappings = MercuryService.GetIp2GeoMappings(new IPAddress[]
                    {
                        srcIp,dstIp
                    });
            foreach (MercuryIpToGeoMapping geo in ip2geoMappings)
            {
                if (geo.Address.ToString() == srcIp.ToString())
                {
                    srcCity = geo.City;
                    srcCountry = geo.CountryName;
                }
                if (geo.Address.ToString() == dstIp.ToString())
                {
                    dstCity = geo.City;
                    dstCountry = geo.CountryName;
                }
            }

            return new string[] { srcCity, srcCountry, dstCity, dstCountry };
        }
         */


        /*
        private MercuryAsTraceroute generateTracerouteAS(ASTraceroutePath asPreviewHops, String dst,
                                                    IPAddress publicIP, IPAddress srcIp, IPAddress dstIp, String srcCity, String srcCountry, String dstCity, String dstCountry )
        {

            MercuryAsTracerouteStats tracerouteASStats = obtainTracerouteStatistics(asPreviewHops);
            //List<MercuryAsTracerouteRelationship> tracerouteASRelationships = new List<MercuryAsTracerouteRelationship>(asPreviewHops.relationships.Values);

            //Let's play with hops!
            List<MercuryAsTracerouteHop> asHopsAux = new List<MercuryAsTracerouteHop>();
            foreach (ASTracerouteHop asHop in asPreviewHops.hops)
            {
                foreach (MercuryAsTracerouteHop hop in asHop.candidates.Values)
                {
                    asHopsAux.Add(hop);
                }
            }

            //Now we search the src and the dst hops
            int srcAs = -1, dstAs = -1;
            String srcAsName = null, dstAsName = null;
            if (asPreviewHops.hops.Count > 0)
            {

                if (asPreviewHops.hops[0].candidates.Count > 0)
                {

                    srcAs = new List<MercuryAsTracerouteHop>(asPreviewHops.hops[0].candidates.Values)[0].AsNumber;
                    srcAsName = new List<MercuryAsTracerouteHop>(asPreviewHops.hops[0].candidates.Values)[0].AsName;
                }

                if (asPreviewHops.hops[asPreviewHops.hops.Count-1].candidates.Count > 0)
                {

                    dstAs = new List<MercuryAsTracerouteHop>(asPreviewHops.hops[asPreviewHops.hops.Count - 1].candidates.Values)[0].AsNumber;
                    dstAsName = new List<MercuryAsTracerouteHop>(asPreviewHops.hops[asPreviewHops.hops.Count - 1].candidates.Values)[0].AsName;
                }

            }


            MercuryAsTraceroute tracerouteAS = new MercuryAsTraceroute(srcAs, srcAsName, srcIp.ToString(), publicIP.ToString(), srcCity, srcCountry,
                dstAs, dstAsName, dstIp.ToString(), dst, dstCity, dstCountry, DateTime.UtcNow, tracerouteASStats);

            tracerouteAS.Hops = asHopsAux;
            //tracerouteAS.Relationships = tracerouteASRelationships;

            return tracerouteAS;
        }
        */


    }
}
