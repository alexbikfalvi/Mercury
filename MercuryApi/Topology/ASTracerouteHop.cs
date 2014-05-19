﻿/* 
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
        public bool IsEqualUnique(ASTracerouteHop hop)
        {
            if (this.asSet.Count != 1) return false;
            if (hop.asSet.Count != 1) return false;
            return this.asSet.First() == hop.asSet.First();
        }

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

        
        public bool isMissing(ASTracerouteHop hop2)
        {
            if (candidates.Count == 0 && hop2.candidates.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isMissingInMiddleSameAS(ASTracerouteHop hop2, ASTracerouteHop hop3)
        {
            if (candidates.Count == 0 && hop2.candidates.Count > 0 & hop3.candidates.Count > 0)
            {
                int matchings = 0;
                foreach (MercuryAsTracerouteHop hop in hop3.candidates.Values)
                {
                    if (hop2.candidates.ContainsKey(hop.AsNumber))
                    {
                        matchings++;
                    }

                }
                if (matchings >= 1)
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


        public bool isEqualUnique(ASTracerouteHop hop2)
        {
            if (candidates.Count == 1 && hop2.candidates.Count == 1)
            {
                if (candidates[0].AsNumber == hop2.candidates[0].AsNumber)
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
            if (candidates.Count == 1 && hop2.candidates.Count == 1)
            {
                foreach (MercuryAsTracerouteHop hop in candidates.Values)
                {

                    if (hop2.candidates.ContainsKey(hop.AsNumber))
                    {
                        return hop2.candidates[hop.AsNumber];
                    }
                }

            }
            return null;
        }


        public bool isEqualUniqueToMultiple(ASTracerouteHop hop2)
        {
            if ((candidates.Count == 1 && hop2.candidates.Count > 1) || (candidates.Count > 1 && hop2.candidates.Count == 1))
            {
                foreach (MercuryAsTracerouteHop hop in candidates.Values)
                {
                    if (hop2.candidates.ContainsKey(hop.AsNumber))
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
            if ((candidates.Count == 1 && hop2.candidates.Count > 1) || (candidates.Count > 1 && hop2.candidates.Count == 1))
            {
                foreach (MercuryAsTracerouteHop hop in candidates.Values)
                {

                    if (hop2.candidates.ContainsKey(hop.AsNumber))
                    {
                        return hop2.candidates[hop.AsNumber];
                    }

                }

            }
            return null;
        }

        public bool isEqualMultipleToMultiple(ASTracerouteHop hop2)
        {
            if (candidates.Count > 1 && hop2.candidates.Count > 1)
            {
                int matchings = 0;
                foreach (MercuryAsTracerouteHop hop in candidates.Values)
                {
                    if (hop2.candidates.ContainsKey(hop.AsNumber))
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

            if (candidates.Count > 1 && hop2.candidates.Count > 1)
            {
                int matchings = 0;
                foreach (MercuryAsTracerouteHop hop in candidates.Values)
                {

                    if (hop2.candidates.ContainsKey(hop.AsNumber))
                    {
                        matchings++;
                        cands[hop.AsNumber] = hop2.candidates[hop.AsNumber];
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
