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
using System.Linq;
using InetApi.Net.Core;
using Mercury.Api;

namespace Mercury.Topology
{
    /// <summary>
    /// A class for a AS traceroute hop.
    /// </summary>
    public class ASTracerouteHop
    {
        private readonly HashSet<ASInformation> asSet = new HashSet<ASInformation>();

        /// <summary>
        /// Creates a new AS traceroute hop for a missing AS.
        /// </summary>
        public ASTracerouteHop()
        {
            this.AsNumber = null;
            this.Flags = ASTracerouteFlags.MissingAs;
        }

        /// <summary>
        /// Creates a new AS traceroute hop from the list of specified AS information.
        /// </summary>
        /// <param name="list">A list of AS information.</param>
        public ASTracerouteHop(IEnumerable<ASInformation> list)
        {
            // Set the list of ASes.
            foreach (ASInformation info in list)
            {
                this.asSet.Add(info);
            }
            if (0 == this.asSet.Count)
            {
                // If the hop has no ASes.
                this.AsNumber = null;
                this.Flags = ASTracerouteFlags.MissingAs;
            }
            else if (1 == this.asSet.Count)
            {
                // If the hop has one AS, get the first AS information.
                ASInformation info = this.asSet.First();
                // If the AS number is positive.
                if (info.AsNumber > 0)
                {
                    // If the hop as one AS.
                    this.AsNumber = info.AsNumber;
                    this.Flags = ASTracerouteFlags.None;
                }
                else
                {
                    this.AsNumber = null;
                    this.Flags = ASTracerouteFlags.MissingAs;
                }
            }
            else
            {
                // If the hop has multiple ASes.
                this.AsNumber = null;
                this.Flags = ASTracerouteFlags.MultipleAs;
            }
        }

        #region Public properties

        /// <summary>
        /// Returns the final AS number for this hop.
        /// </summary>
        public int? AsNumber { get; private set; }
        /// <summary>
        /// Returns the flags for this AS hop.
        /// </summary>
        public ASTracerouteFlags Flags { get; private set; }
        /// <summary>
        /// Returns true if this hop can be successfully solved to an AS number.
        /// </summary>
        public bool IsSuccessful { get { return this.Flags.IsSuccessful(); } }
        /// <summary>
        /// Gets the set of AS information for this hop.
        /// </summary>
        public HashSet<ASInformation> AsSet { get { return this.asSet; } }
        /// <summary>
        /// Gets the corresponding IP data.
        /// </summary>
        public MultipathTracerouteData IpData { get; internal set; }

        #endregion

        #region Static methods

        /// <summary>
        /// Compares two AS traceroute hops for equality.
        /// </summary>
        /// <param name="left">The left hop.</param>
        /// <param name="right">The right hop.</param>
        /// <returns><b>True</b> if the hops are equal, <b>false</b> otherwise.</returns>
        public static bool operator==(ASTracerouteHop left, ASTracerouteHop right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null)) return true;
            if (object.ReferenceEquals(left, null)) return false;
            if (object.ReferenceEquals(right, null)) return false;
            return (left.IsSuccessful == right.IsSuccessful) && (left.AsNumber == right.AsNumber);
        }

        /// <summary>
        /// Compare two AS traceroute hops for inequality.
        /// </summary>
        /// <param name="left">The first path.</param>
        /// <param name="right">The second path.</param>
        /// <returns><b>True</b> if the hops are different, <b>false</b> otherwise.</returns>
        public static bool operator !=(ASTracerouteHop left, ASTracerouteHop right)
        {
            return !(left == right);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Compares this AS traceroute hops for equality.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><b>True</b> if the path is equal, <b>false</b> otherwise.</returns>
        public override bool Equals(object path)
        {
            return this == (path as ASTracerouteHop);
        }

        /// <summary>
        /// Gets the hash code for this AS traceroute hop.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return this.AsNumber.GetHashCode() ^ this.IsSuccessful.GetHashCode();
        }

        /// <summary>
        /// Verifies if the current hop is equally unique to the specified hop.
        /// </summary>
        /// <param name="hop">The other hop.</param>
        /// <returns><b>True</b> if the hops are equally unique, <b>false</b> otherwise.</returns>
        //public bool IsEqualUnique(ASTracerouteHop hop)
        //{
        //    if (this.asSet.Count != 1) return false;
        //    if (hop.asSet.Count != 1) return false;
        //    return this.asSet.First() == hop.asSet.First();
        //}

        /*
        /// <summary>
        /// Verifies if the current hop is equally unique to multiple to the specified hop.
        /// </summary>
        /// <param name="hop">The hop.</param>
        /// <param name="info">The equal AS information.</param>
        /// <returns><b>True</b> if the hops are equal, <b>false</b> otherwise.</returns>
        public bool IsEqualUniqueToMultiple(ASTracerouteHop hop, out ASInformation info)
        {
            HashSet<ASInformation> set;
            info = null;
            if (this.asSet.Count == 1)
            {
                info = this.asSet.First();
                set = hop.asSet;
            }
            else if (hop.asSet.Count == 1)
            {
                info = hop.asSet.First();
                set = this.asSet;
            }
            else return false;

            return set.Contains(info, new IEqualityComparer<ASInformation>());
        }
        */
        
        public bool isMissing(ASTracerouteHop hopLeft)
        {
            if (this.asSet.Count == 0 && hopLeft.asSet.Count == 0) return true;
            else return false;

        }

        public bool isMissingInMiddleSameAS(ASTracerouteHop hopLeft, ASTracerouteHop hopRight)
        {
            //Workaround to solve searches
            Dictionary<int, ASInformation> candidatesLeft = new Dictionary<int, ASInformation>();
            foreach (ASInformation candidate in hopLeft.asSet)
            {
                candidatesLeft[candidate.AsNumber] = candidate;
            }
            Dictionary<int, ASInformation> candidatesRight = new Dictionary<int, ASInformation>();
            foreach (ASInformation candidate in hopRight.asSet)
            {
                candidatesRight[candidate.AsNumber] = candidate;
            }

            if (asSet.Count == 0 && hopLeft.asSet.Count > 0 & hopRight.asSet.Count > 0)
            {
                int matchings = 0;
                foreach (ASInformation hop in hopRight.asSet)
                {
                    if (candidatesLeft.ContainsKey(hop.AsNumber))
                    {
                        matchings++;
                    }
                }
                if (matchings >= 1) return true;
                else return false;
            }
            else return false;

        }


        public bool IsEqualUnique(ASTracerouteHop hopLeft, out ASInformation info)
        {

            info = null;
            if (this.asSet.Count != 1) return false;
            if (hopLeft.asSet.Count != 1) return false;

            if (this.asSet.First() == hopLeft.asSet.First())
            {
                //Prioritize for IXPs
                info = this.asSet.First();
                if (this.asSet.First().Type == ASInformation.AsType.Ixp && this.asSet.First().AsNumber > 0) info = this.asSet.First();
                if (hopLeft.asSet.First().Type == ASInformation.AsType.Ixp && hopLeft.asSet.First().AsNumber > 0) info = hopLeft.asSet.First();
                return true;

            }
            else return false;
        }
        /*
        public bool isEqualUnique(ASTracerouteHop hop2)
        {
            if (asSet.Count == 1 && hop2.asSet.Count == 1)
            {
                if (asSet[0].AsNumber == hop2.asSet[0].AsNumber)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public MercuryAsTracerouteHop getEqualUnique(ASTracerouteHop hop2)
        {
            if (asSet.Count == 1 && hop2.asSet.Count == 1)
            {
                foreach (MercuryAsTracerouteHop hop in asSet.Values)
                {

                    if (hop2.asSet.ContainsKey(hop.AsNumber))
                    {
                        return hop2.asSet[hop.AsNumber];
                    }
                }

            }
            return null;
        }
        */

        public bool IsEqualUniqueToMultiple(ASTracerouteHop hopLeft, out ASInformation info)
        {
            info = null;
            Dictionary<int, ASInformation> candidates = new Dictionary<int, ASInformation>();
            foreach (ASInformation candidate in this.asSet)
            {
                candidates[candidate.AsNumber] = candidate;
            }
            Dictionary<int, ASInformation> candidatesLeft = new Dictionary<int, ASInformation>();
            foreach (ASInformation candidate in hopLeft.asSet)
            {
                candidatesLeft[candidate.AsNumber] = candidate;
            }

            if ((this.asSet.Count == 1 && hopLeft.asSet.Count > 1) || (this.asSet.Count > 1 && hopLeft.asSet.Count == 1))
            {
                foreach (ASInformation hop in this.asSet)
                {
                    if (candidatesLeft.ContainsKey(hop.AsNumber))
                    {
                        //Prioritize for IXPs
                        info = candidates[hop.AsNumber];
                        if (candidates[hop.AsNumber].Type == ASInformation.AsType.Ixp && candidates[hop.AsNumber].AsNumber > 0) info = candidates[hop.AsNumber];
                        if (candidatesLeft[hop.AsNumber].Type == ASInformation.AsType.Ixp && candidatesLeft[hop.AsNumber].AsNumber > 0) info = candidatesLeft[hop.AsNumber];
                        return true;
                    }
                }
                return false;
            }
            else return false;
        }
        /*
        public bool isEqualUniqueToMultiple(ASTracerouteHop hop2)
        {
            if ((asSet.Count == 1 && hop2.asSet.Count > 1) || (asSet.Count > 1 && hop2.asSet.Count == 1))
            {
                foreach (MercuryAsTracerouteHop hop in asSet.Values)
                {
                    if (hop2.asSet.ContainsKey(hop.AsNumber))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public MercuryAsTracerouteHop getEqualUniqueToMultiple(ASTracerouteHop hop2)
        {
            if ((asSet.Count == 1 && hop2.asSet.Count > 1) || (asSet.Count > 1 && hop2.asSet.Count == 1))
            {
                foreach (MercuryAsTracerouteHop hop in asSet.Values)
                {

                    if (hop2.asSet.ContainsKey(hop.AsNumber))
                    {
                        return hop2.asSet[hop.AsNumber];
                    }

                }

            }
            return null;
        }
        */

        public bool IsEqualMultipleToMultiple(ASTracerouteHop hopLeft, out ASInformation info)
        {
            info = null;
            HashSet<ASInformation> matches = new HashSet<ASInformation>();
            Dictionary<int, ASInformation> candidates = new Dictionary<int, ASInformation>();
            foreach (ASInformation candidate in this.asSet)
            {
                candidates[candidate.AsNumber] = candidate;
            }
            Dictionary<int, ASInformation> candidatesLeft = new Dictionary<int, ASInformation>();
            foreach (ASInformation candidate in hopLeft.asSet)
            {
                candidatesLeft[candidate.AsNumber] = candidate;
            }

            if ((this.asSet.Count > 1 && hopLeft.asSet.Count > 1))
            {
                int matchings = 0;
                foreach (ASInformation hop in this.asSet)
                {
                    if (candidatesLeft.ContainsKey(hop.AsNumber))
                    {
                        matchings++;
                        matches.Add(candidates[hop.AsNumber]);
                    }
                }
                if (matchings > 1)
                {
                    foreach (ASInformation hop in matches)
                    {
                        //Prioritize for IXPs
                        info = candidates[hop.AsNumber];
                        if (candidates[hop.AsNumber].Type == ASInformation.AsType.Ixp && candidates[hop.AsNumber].AsNumber > 0) info = candidates[hop.AsNumber];
                        if (candidatesLeft[hop.AsNumber].Type == ASInformation.AsType.Ixp && candidatesLeft[hop.AsNumber].AsNumber > 0) info = candidatesLeft[hop.AsNumber];
                        return true;
                    }
                    return false;
                }
                else return false;
            }
            else return false;
        }

        /*
        public bool isEqualMultipleToMultiple(ASTracerouteHop hop2)
        {
            if (asSet.Count > 1 && hop2.asSet.Count > 1)
            {
                int matchings = 0;
                foreach (MercuryAsTracerouteHop hop in asSet.Values)
                {
                    if (hop2.asSet.ContainsKey(hop.AsNumber))
                    {
                        matchings++;
                    }
                }
                if (matchings > 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //We might return just one AS, instead of all the list again...
        public MercuryAsTracerouteHop getEqualMultipleToMultiple(ASTracerouteHop hop2)
        {
            Dictionary<int, MercuryAsTracerouteHop> cands = new Dictionary<int, MercuryAsTracerouteHop>();

            if (asSet.Count > 1 && hop2.asSet.Count > 1)
            {
                int matchings = 0;
                foreach (MercuryAsTracerouteHop hop in asSet.Values)
                {

                    if (hop2.asSet.ContainsKey(hop.AsNumber))
                    {
                        matchings++;
                        cands[hop.AsNumber] = hop2.asSet[hop.AsNumber];
                    }

                }
                if (matchings > 1)
                {
                    List<MercuryAsTracerouteHop> asHops = new List<MercuryAsTracerouteHop>(cands.Values);
                    return asHops[0];
                }
            }
            return null;
        }
        */

        #endregion
    }
}
