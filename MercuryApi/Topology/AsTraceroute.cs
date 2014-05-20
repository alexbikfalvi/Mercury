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
		public ASTracerouteResult Run(String destination, MultipathTracerouteResult traceroute, CancellationToken cancel, ASTracerouteCallback callback)
		{
            // Create the AS traceroute result.
            ASTracerouteResult result = new ASTracerouteResult(traceroute, callback);

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


            //Get the destination URL
            destination = "example.com";//traceroute.Stat

            // Add the public address and remote address. 
            MercuryLocalInformation localInformation = MercuryService.GetLocalInformation();
            addresses.Add(localInformation.Address);
            addresses.Add(traceroute.RemoteAddress);

            // We pre-load the list of IP addresses to AS information and we stored it in cache.
            this.cache.Update(addresses);

            // Get the local public AS information.
            List<ASInformation> sourceAsList = this.cache.Get(localInformation.Address);
            // Get the remote AS information.
            List<ASInformation> destinationAsList = this.cache.Get(traceroute.RemoteAddress);


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
                                result.PathsStep1[(byte)algorithm, flow, attempt].AddHop(ases).IpData = traceroute.Data[(byte)algorithm, flow, attempt, ttl];
                            }
                            else
                            {
                                // Add a new hop for a missing AS.
                                result.PathsStep1[(byte)algorithm, flow, attempt].AddHop().IpData = traceroute.Data[(byte)algorithm, flow, attempt, ttl];
                            }
                        }
                    }
                }
            }

            // Call the callback method.
            result.Callback(ASTracerouteState.StateType.Step1);

            /*
             * STEP 2: Aggregate hops for the same path.
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
                        // Add the public source AS to the first position.
                        result.PathsStep1[(byte)algorithm, flow, attempt].AddSource(sourceAsList);
                        // Add the destination AS to the last position.
                        result.PathsStep1[(byte)algorithm, flow, attempt].AddDestination(destinationAsList);
                        // Aggregate the hops.
                        result.PathsStep2[(byte)algorithm, flow, attempt] = this.AggregateHops(result.PathsStep1[(byte)algorithm, flow, attempt]);
                    }
                }
            }

            // Call the callback method.
//            result.Callback(ASTracerouteState.StateType.Step2);

            /*
             * STEP 3: Aggregate attempt paths for the same flow.
             */

            // Compare the paths for different attempts in the same flow.
            ASTraceroutePath.HashSet attempts = new ASTraceroutePath.HashSet();
            foreach (MultipathTracerouteResult.ResultAlgorithm algorithm in traceroute.Algorithms)
            {
                for (byte flow = 0; flow < traceroute.Settings.AttemptsPerFlow; flow++)
                {
                    // Clear the attempt.
                    attempts.Clear();
                    for (byte attempt = 0; attempt < traceroute.Settings.AttemptsPerFlow; attempt++)
                    {
                        // Add the attempt to the list of attempts.
                        attempts.Add(result.PathsStep2[(byte)algorithm, flow, attempt]);
                    }
                    // If there is only one path.
                    if (attempts.Count == 1)
                    {
                        // Add the path to the result.
                        result.PathsStep3[(byte)algorithm, flow] = this.IdentifyLoops(attempts.First());                       
                    }
                    else
                    {
                        // We have distinct attempts, so we mark it with the flag.
                        result.PathsStep3[(byte)algorithm, flow] = new ASTraceroutePath(ASTracerouteFlags.FlowDistinctAttempts);                       
                    }
                }
            }

            // Call the callback method.
//          result.Callback(ASTracerouteState.StateType.Step3);

            /*
             * Step 4: Save unique flows.
             */
            
            // Create the list of unique flows.
            ASTraceroutePath.HashSet flows = new ASTraceroutePath.HashSet();
            foreach (MultipathTracerouteResult.ResultAlgorithm algorithm in traceroute.Algorithms)
            {
                for (byte flow = 0; flow < traceroute.Settings.AttemptsPerFlow; flow++)
                {
                    flows.Add(result.PathsStep3[(byte)algorithm, flow]);
                }
            }
            result.PathsStep4 = flows.ToArray();

            // Call the callback method.
//            result.Callback(ASTracerouteState.StateType.Step4);


            /*
             * Step 5: Finally we generate and upload the MercuryAsTraceroute(es) to the Mercury Platform
             */
            List<MercuryAsTraceroute> tracerouteASes = new List<MercuryAsTraceroute>();
            foreach (ASTraceroutePath path in result.PathsStep4)
            {
                tracerouteASes.Add( generateTracerouteAS(path, destination, localInformation.Address, traceroute.LocalAddress, traceroute.RemoteAddress) );
            }
            String mercuryPlatformResponse = MercuryService.addTracerouteASes(tracerouteASes);

            

            return null;
		}

        #region Private methods

        /// <summary>
        /// Aggregates equals AS hops for the specified AS path. It also adds flags to hops
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The aggregated AS path.</returns>
        private ASTraceroutePath AggregateHops(ASTraceroutePath path)
        {
            // Create the resulting path.
            ASTraceroutePath result = new ASTraceroutePath();
                
            // For each hop
            for (int hop = 1; hop < path.Hops.Count; hop++)
            {
                if (result.Hops.Count > 0)
                {
                    if (!path.Hops[hop].IsMissing(result.Hops[result.Hops.Count - 1])) //We compare with the last index of the aux list
                    {
                        ASInformation asInformation = null;
                        if (!path.Hops[hop].IsEqualUnique(result.Hops[result.Hops.Count - 1], out asInformation))
                        {
                            if (path.Hops[hop].IsEqualUniqueToMultiple(result.Hops[result.Hops.Count - 1], out asInformation))
                            {
                                result.Hops.RemoveAt(result.Hops.Count - 1);//We remove the previous
                                ASTracerouteHop thop = new ASTracerouteHop();
                                thop.AsSet.Add(asInformation);
                                thop.AsNumber = asInformation.AsNumber;
                                thop.Flags |= ASTracerouteFlags.MultipleAs;
                                result.Flags |= thop.Flags | ASTracerouteFlags.MultipleAsEqualNeighbor; //We scale up the hop flag to the path
                                result.Hops.Add(thop);

                            }
                            else
                            {
                                if (path.Hops[hop].IsEqualMultipleToMultiple(result.Hops[result.Hops.Count - 1], out asInformation))
                                {
                                    result.Hops.RemoveAt(result.Hops.Count - 1);//We remove the previous
                                    ASTracerouteHop thop = new ASTracerouteHop();
                                    thop.AsSet.Add(asInformation);
                                    thop.AsNumber = asInformation.AsNumber;
                                    thop.Flags |= ASTracerouteFlags.MultipleAs;
                                    result.Flags |= thop.Flags | ASTracerouteFlags.MultipleAsEqualNeighbor; //We scale up the hop flag to the path
                                    result.Hops.Add(thop);

                                }
                                else //if different ases (AS0-AS1), different multiple ases (AS0/AS1 - AS2/AS3) or missing-AS
                                {
                                    //We check for missings in the middle of same AS
                                    int j = 0;
                                    for (j = hop; j < path.Hops.Count; j++)
                                    {
                                        if (path.Hops[j].AsSet.Count > 0)
                                        {
                                            break;
                                        }
                                    }
                                    if (path.Hops[hop].IsMissingInMiddleSameAS(result.Hops[result.Hops.Count - 1], path.Hops[j])) //If missing in the middle of same as
                                    {
                                        hop = j;
                                        result.Flags |= ASTracerouteFlags.MissingHopInsideAs;
                                    }
                                    else //No missing in the middle of same AS
                                    {
                                        if (path.Hops[hop].AsSet.Count > 1) result.Flags = result.Flags | ASTracerouteFlags.MultipleAsDifferentNeighbor;
                                        //if (path.Hops[hop].AsSet.Count == 1) result.Flags = result.Flags | ASTracerouteFlags.None;
                                        if (path.Hops[hop].AsSet.Count == 1) result.Flags = result.Flags | ASTracerouteFlags.MissingHopEdgeAs;
                                        result.Hops.Add(path.Hops[hop]);
                                    }

                                }
                            }
                        }
                        else
                        {
                            result.Hops.RemoveAt(result.Hops.Count - 1);//We remove the previous
                            ASTracerouteHop thop = new ASTracerouteHop();
                            thop.AsSet.Add(asInformation);
                            thop.AsNumber = asInformation.AsNumber;
                            thop.Flags = ASTracerouteFlags.None;
                            result.Hops.Add(thop);
                        }
                    }

                }
                else //if result list is empty, we are in first hop
                {
                    if (path.Hops[hop].IsMissing(path.Hops[hop - 1]))
                    {
                        result.Hops.Add(new ASTracerouteHop());
                    }
                    else
                    {
                        ASInformation asInformation = null;
                        if (path.Hops[hop].IsEqualUnique(path.Hops[hop - 1], out asInformation))
                        {
                            //we do not need to remove cause we are in first position
                            ASTracerouteHop thop = new ASTracerouteHop();
                            thop.AsSet.Add(asInformation);
                            thop.AsNumber = asInformation.AsNumber;
                            thop.Flags = ASTracerouteFlags.None;
                            result.Hops.Add(thop);
                        }
                        else
                        {
                            if (path.Hops[hop].IsEqualUniqueToMultiple(path.Hops[hop - 1], out asInformation))
                            {
                                //we do not need to remove cause we are in first position
                                ASTracerouteHop thop = new ASTracerouteHop();
                                thop.AsSet.Add(asInformation);
                                thop.AsNumber = asInformation.AsNumber;
                                thop.Flags = ASTracerouteFlags.MultipleAs;
                                result.Flags = result.Flags | thop.Flags | ASTracerouteFlags.MultipleAsEqualNeighbor; //We scale up the hop flag to the path
                                result.Hops.Add(thop);

                            }
                            else
                            {
                                if (path.Hops[hop].IsEqualMultipleToMultiple(path.Hops[hop - 1], out asInformation))
                                {
                                    //we do not need to remove cause we are in first position
                                    ASTracerouteHop thop = new ASTracerouteHop();
                                    thop.AsSet.Add(asInformation);
                                    thop.AsNumber = asInformation.AsNumber;
                                    thop.Flags = ASTracerouteFlags.MultipleAs;
                                    result.Flags = result.Flags | thop.Flags | ASTracerouteFlags.MultipleAsEqualNeighbor; //We scale up the hop flag to the path
                                    result.Hops.Add(thop);
                                }
                                else //is differentes ases (AS0-AS1) or missing-AS
                                {
                                    if (path.Hops[hop].AsSet.Count > 1) result.Flags = result.Flags | ASTracerouteFlags.MultipleAsDifferentNeighbor;
                                    if (path.Hops[hop].AsSet.Count == 1) result.Flags = result.Flags | ASTracerouteFlags.None;
                                    if (path.Hops[hop].AsSet.Count == 0) result.Flags = result.Flags | ASTracerouteFlags.MissingSource;
                                    result.Hops.Add(path.Hops[hop]);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Identifies loops in the current path and activates flags if it finds it
        /// </summary>
        /// <param name="path">The AS path.</param>
        /// <returns>The AS path.</returns>
        private ASTraceroutePath IdentifyLoops(ASTraceroutePath path)
        {
            for (int hop = 1; hop < path.Hops.Count-1; hop++)
            {
                //If loop found, we add the Flag
                if (path.Hops[hop].IsOtherAsInMiddleSameAs(path.Hops[hop - 1], path.Hops[hop + 1]))
                {
                    path.Flags = path.Flags | ASTracerouteFlags.LoopPath;
                    break;
                }
            }
            // Return the path.
            return path;
        }


        /// <summary>
        /// Processes all the AS Relationships in a path
        /// </summary>
        /// <param name="path">The AS path.</param>
        /// <returns>The AS path.</returns>
        private ASTraceroutePath obtainASRelationships(ASTraceroutePath path)
        {
            for (byte i = 0; i < path.Hops.Count() - 1; i++) //We end before the last hop
            {
                if (path.Hops.ElementAt(i).AsSet.Count() != 0 && path.Hops.ElementAt(i + 1).AsSet.Count() != 0) //If NOT missing hops
                {
                    MercuryAsTracerouteRelationship rel = null;
                    int as0 = (int)path.Hops.ElementAt(i).AsNumber;
                    int as1 = (int)path.Hops.ElementAt(i + 1).AsNumber;

                    //If is IXP
                    if (path.Hops.ElementAt(i + 1).AsSet.First().Type == ASInformation.AsType.Ixp)
                    {
                        rel = new MercuryAsTracerouteRelationship(MercuryAsTracerouteRelationship.RelationshipType.InternerExchangePoint, as0, as1, i);
                    }
                    else
                    {
                        rel = MercuryService.GetAsRelationship(as0, as1);
                        rel.Hop = i;
                    }
                    path.relationships.Add(rel);
                }
                /*THIS IS NEW!! GUESSING RELs with MISSING HOPs in the MIDDLE*/
                if (path.Hops.ElementAt(i).AsSet.Count() != 0 && path.Hops.ElementAt(i + 1).AsSet.Count() == 0) //Here we try to guess relationships with a missing hop in the middle
                {
                    if (i + 2 < path.Hops.Count()) //We check that we not overpass the limit
                    {
                        MercuryAsTracerouteRelationship rel = null;
                        int as0 = (int)path.Hops.ElementAt(i).AsNumber;
                        int as1 = (int)path.Hops.ElementAt(i + 2).AsNumber;

                        //If is IXP
                        if (path.Hops.ElementAt(i + 2).AsSet.First().Type == ASInformation.AsType.Ixp)
                        {
                            rel = new MercuryAsTracerouteRelationship(MercuryAsTracerouteRelationship.RelationshipType.InternerExchangePoint, as0, as1, i);
                        }
                        else
                        {
                            rel = MercuryService.GetAsRelationship(as0, as1);
                            rel.Hop = i;
                        }
                        path.relationships.Add(rel);
                        i++; //we increment index because we jump a missing hop
                    }
                }
            }
            return path;
        }


        /// <summary>
        /// Generates the statistics of a path
        /// </summary>
        /// <param name="path">The AS path.</param>
        /// <returns>The AS Traceroute Stats.</returns>
        private MercuryAsTracerouteStats obtainTracerouteStatistics(ASTraceroutePath path)
        {
            int asHops = 0;
            int c2pRels = 0,p2pRels = 0,p2cRels = 0,s2sRels = 0,ixpRels = 0,nfRels = 0;
            bool completed = false;
            int flags = (int)path.Flags;

            //We set completed to true if Flags is...
            if (flags > 0x0) completed = true;

            //We count the asHops - 1
            asHops = path.Hops.Count() - 1;

            //We count the asRelationships
            foreach (MercuryAsTracerouteRelationship rel in path.relationships)
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


        /// <summary>
        /// Obtains the geomapping for the source and destination IP Address of a path
        /// </summary>
        /// <param name="srcIp">The source IP address.</param>
        /// <param name="dstIp">The destination IP address.</param>
        /// <returns>The an array with 4 positions. [0] srcCity [1]srcCountry [2]dstCity [3] dstCountry</returns>
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



        /// <summary>
        /// Generates the Traceroute AS object to be sent to the Mercury Platform
        /// </summary>
        /// <param name="path">The AS path.</param>
        /// <param name="dst">The URL destination (e.g. upf.edu).</param>
        /// <param name="publicIP">The public IP address.</param>
        /// <param name="srcIp">The source IP address of the host (usually a private IP address).</param>
        /// <param name="dstIp">The destination IP address of the URL destination.</param>
        /// <returns>The AS path.</returns>
        private MercuryAsTraceroute generateTracerouteAS(ASTraceroutePath path, String dst,
                                                    IPAddress publicIP, IPAddress srcIp, IPAddress dstIp )
        {

            //Let's play with hops!
            List<MercuryAsTracerouteHop> asHopsAux = new List<MercuryAsTracerouteHop>();
            byte hopCount = 0;
            foreach (ASTracerouteHop asHop in path.Hops)
            {
                if (asHop.AsSet.Count > 0)
                {
                    foreach (ASInformation info in asHop.AsSet)
                    {
                        MercuryAsTracerouteHop hop = new MercuryAsTracerouteHop(hopCount, info);
                        asHopsAux.Add(hop);
                    }
                }
                else
                {
                    //Here we add missing hops
                    asHopsAux.Add(new MercuryAsTracerouteHop(hopCount));
                }
                hopCount++;
            }

            //Now we search the src and the dst hops
            int srcAs = -1, dstAs = -1;
            String srcAsName = null, dstAsName = null;
            if (path.Hops.Count() > 0)
            {
                if (path.Hops.First().AsSet.Count() > 0)
                {
                    srcAs = (int)path.Hops.First().AsNumber;
                    srcAsName = path.Hops.First().AsSet.First().AsName;
                }

                if (path.Hops.Last().AsSet.Count() > 0)
                {
                    dstAs = (int)path.Hops.Last().AsSet.First().AsNumber;
                    dstAsName = path.Hops.Last().AsSet.First().AsName;
                }
            }

            //Now we obtain the geomappings
            String[] geoMappings = getGeoMappings(publicIP, dstIp);
            String srcCity = geoMappings[0]; String srcCountry = geoMappings[1]; String dstCity = geoMappings[2]; String dstCountry = geoMappings[3];

            //We obtain the traceroute AS Relationship before processing Stats
            path = obtainASRelationships(path);
            //We obtain the traceroute Stats
            MercuryAsTracerouteStats tracerouteASStats = obtainTracerouteStatistics(path);

            //We create the tracerouteAS object
            MercuryAsTraceroute tracerouteAS = new MercuryAsTraceroute(srcAs, srcAsName, srcIp.ToString(), publicIP.ToString(), srcCity, srcCountry,
                dstAs, dstAsName, dstIp.ToString(), dst, dstCity, dstCountry, DateTime.UtcNow, tracerouteASStats);

            //We add the hops
            tracerouteAS.Hops = asHopsAux;
            //We add the relationships
            tracerouteAS.Relationships = path.relationships;

            return tracerouteAS;
        }

        #endregion

    }
}
