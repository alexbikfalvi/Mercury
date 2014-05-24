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

        #region Public properties

        /// <summary>
        /// Gets the AS traceroute cache.
        /// </summary>
        public ASTracerouteCache Cache { get { return this.cache; } }

        #endregion

        #region Public methods

        /// <summary>
		/// Runs an AS-level traceroute starting from the specified IP-level multipath traceroute.
		/// </summary>
		/// <param name="traceroute">The IP-level multipath traceroute.</param>
		/// <param name="cancel">The cancellation token.</param>
		/// <param name="callback">The callback methods.</param>
		/// <returns>The AS-level traceroute.</returns>
		public ASTracerouteResult Run(MultipathTracerouteResult traceroute, CancellationToken cancel, ASTracerouteCallback callback)
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

            // Add the public address and remote address. 
            MercuryLocalInformation localInformation = MercuryService.GetLocalInformation();
            addresses.Add(localInformation.Address);
            addresses.Add(traceroute.RemoteAddress);

            // We pre-load the list of IP addresses to AS information and we stored it in cache.
            this.cache.Update(addresses);

            // Get the local public AS information.
            List<ASInformation> sourceAsList = this.cache.GetUpdate(localInformation.Address);
            // Get the remote AS information.
            List<ASInformation> destinationAsList = this.cache.GetUpdate(traceroute.RemoteAddress);


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
                                List<ASInformation> ases = this.cache.GetCache(traceroute.Data[(byte)algorithm, flow, attempt, ttl].Address);

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
            result.Callback(ASTracerouteState.StateType.Step2);

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
                        result.PathsStep3[(byte)algorithm, flow] = this.IdentifyLoops(attempts.First()); //identify loops
                        result.PathsStep3[(byte)algorithm, flow] = this.CorrectFirstLoop(attempts.First()); //Corect the first loop
                    }
                    else if (attempts.Count == 2) // Temporary solution... we choose the attempt with the best flags
                    {
                        ASTraceroutePath tempPath = null;
                        if (attempts.First().Flags > attempts.Last().Flags)
                        {
                            tempPath = attempts.Last();
                        }
                        else
                        {
                            tempPath = attempts.First();
                        }
                        // Add the path to the result.
                        result.PathsStep3[(byte)algorithm, flow] = this.IdentifyLoops(tempPath); //identify loops
                        result.PathsStep3[(byte)algorithm, flow] = this.CorrectFirstLoop(tempPath); //Corect the first loop
                    }
                    else
                    {
                        // We have distinct attempts, so we mark it with the flag.
                        result.PathsStep3[(byte)algorithm, flow] = new ASTraceroutePath(ASTracerouteFlags.FlowDistinctAttempts);   
                    }
                }
            }

            // Call the callback method.
            result.Callback(ASTracerouteState.StateType.Step3);

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
            result.Callback(ASTracerouteState.StateType.Step4);

            return result;
		}

        #endregion

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

            //If dstAS is equal to srcAS we have servers inside AS (Google/Akamai)
            if (path.Hops.First().AsNumber.HasValue && path.Hops.Last().AsNumber.HasValue)
            {
                if (path.Hops.First().AsNumber == path.Hops.Last().AsNumber)
                {
                    result.Hops.Add(path.Hops.First());
                    return result;
                }
            }
    
            //SPECIAL CASE: If there are only two valid hops in path
            if (path.Hops.Count() == 2)
            {
                if (path.Hops.First().AsNumber.HasValue && path.Hops.Last().AsNumber.HasValue)
                {
                    if (path.Hops.First().AsNumber != path.Hops.Last().AsNumber)
                    {
                        return path;
                    }
                }
            }

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
                                    if(j==path.Hops.Count)
                                    {
                                        result.Flags |= ASTracerouteFlags.MissingDestination;
                                    }
                                    else if (path.Hops[hop].IsMissingInMiddleSameAS(result.Hops[result.Hops.Count - 1], path.Hops[j])) //If missing in the middle of same as
                                    {
                                        hop = j;
                                        result.Flags |= ASTracerouteFlags.MissingHopInsideAs;
                                    }
                                    else //No missing in the middle of same AS
                                    {
                                        if (path.Hops[hop].AsSet.Count > 1)
                                        {
                                            result.Flags = result.Flags | ASTracerouteFlags.MultipleAsDifferentNeighbor;
                                            path.Hops[hop].AsNumber = path.Hops[hop].AsSet.First().AsNumber; //We set the AsNumber with the first candidate. We must improve this :-(
                                        }
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
                                    if (path.Hops[hop-1].AsSet.Count > 1)
                                    {
                                        result.Flags = result.Flags | ASTracerouteFlags.MultipleAsDifferentNeighbor;
                                        path.Hops[hop - 1].AsNumber = path.Hops[hop - 1].AsSet.First().AsNumber; //We set the AsNumber with the first candidate . We must improve this :-(
                                    }
                                    //if (path.Hops[hop-1].AsSet.Count == 1) result.Flags = result.Flags | ASTracerouteFlags.None;
                                    if (path.Hops[hop-1].AsSet.Count == 0) result.Flags = result.Flags | ASTracerouteFlags.MissingSource;
                                    result.Hops.Add(path.Hops[hop-1]);
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
            HashSet<int> asNumbers = new HashSet<int>();
            foreach(ASTracerouteHop hop in path.Hops ){
                if (hop.AsNumber.HasValue)
                {
                    if(asNumbers.Contains((int)hop.AsNumber))
                    {
                        path.Flags = path.Flags | ASTracerouteFlags.LoopPath;
                        break;
                    }
                    else
                    {
                        asNumbers.Add((int)hop.AsNumber);
                    }
                }
            }
            return path;

            //for (int hop = 1; hop < path.Hops.Count-1; hop++)
            //{
            //    //If loop found, we add the Flag
            //    if (path.Hops[hop].IsOtherAsInMiddleSameAs(path.Hops[hop - 1], path.Hops[hop + 1]))
            //    {
            //        path.Flags = path.Flags | ASTracerouteFlags.LoopPath;
            //        break;
            //    }
            //}
            //// Return the path.
            //return path;
        }

        /// <summary>
        /// It is common a loop in local ISP, so we correct it
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private ASTraceroutePath CorrectFirstLoop(ASTraceroutePath path)
        {
            if (path.Hops.Count <= 2) return path;

            int firstIndex = path.Hops[0].AsNumber.HasValue ? 0 : 1;

            for (int index = firstIndex + 2; index < path.Hops.Count; index++)
            {
                if (path.Hops[index].AsNumber.HasValue ? path.Hops[index].AsNumber == path.Hops[firstIndex].AsNumber : false)
                {
                    path.Hops.RemoveAt(index);
                }
            }
            return path;
        }
        
        #endregion
    }
}
