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
using Mercury.Api;

namespace Mercury.Topology
{
    /// <summary>
    /// A class representing an AS traceroute path.
    /// </summary>
    public class ASTraceroutePath
    {
        /// <summary>
        /// The equality comparer for an AS traceroute path.
        /// </summary>
        public class EqualityComparer : IEqualityComparer<ASTraceroutePath>
        {  
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="first">The first path.</param>
            /// <param name="second">The second path.</param>
            /// <returns><b>True</b> if the specified objects are equal; otherwise, <b>false</b>.</returns>
            public bool Equals(ASTraceroutePath first, ASTraceroutePath second)
            {
                if (first.hops.Count != second.hops.Count) return false;
                if (first.IsSuccessful != second.IsSuccessful) return false;
                for (int index = 0; index < first.hops.Count; index++)
                    if (first.hops[index] != second.hops[index]) return false;
                return true;
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>A hash code for the specified path.</returns>
            public int GetHashCode(ASTraceroutePath path)
            {
                int hashCode = path.IsSuccessful.GetHashCode();
                foreach (ASTracerouteHop hop in path.hops)
                    hashCode ^= hop.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// A hash set for AS traceroute path.
        /// </summary>
        public class HashSet : HashSet<ASTraceroutePath>
        {
            /// <summary>
            /// Creates a new hash set instance.
            /// </summary>
            public HashSet()
                : base(new EqualityComparer())
            {
            }
        }

        private readonly List<ASTracerouteHop> hops = new List<ASTracerouteHop>();

        public List<MercuryAsTracerouteRelationship> relationships = new List<MercuryAsTracerouteRelationship>();

        /// <summary>
        /// Creates an empty AS traceroute path.
        /// </summary>
        public ASTraceroutePath()
        {
            this.Flags = ASTracerouteFlags.None;
        }

        /// <summary>
        /// Creates an empty AS traceroute path with the specified flags.
        /// </summary>
        /// <param name="flags">The flags.</param>
        public ASTraceroutePath(ASTracerouteFlags flags)
        {
            this.Flags = flags;
        }

        #region Public properties

        /// <summary>
        /// Gets the flags for this AS traceroute path.
        /// </summary>
        public ASTracerouteFlags Flags { get; set; }
        /// <summary>
        /// Returns whether this AS path is successful.
        /// </summary>
        public bool IsSuccessful { get { return this.Flags.IsSuccessful(); } }
        /// <summary>
        /// Gets the list of hops for the path.
        /// </summary>
        public List<ASTracerouteHop> Hops { get { return this.hops; } }


        #endregion

        #region Static methods

        /// <summary>
        /// Compare two AS traceroute paths for equality.
        /// </summary>
        /// <param name="left">The first path.</param>
        /// <param name="right">The second path.</param>
        /// <returns><b>True</b> if the paths are equal, <b>false</b> otherwise.</returns>
        public static bool operator==(ASTraceroutePath left, ASTraceroutePath right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null)) return true;
            if (object.ReferenceEquals(left, null)) return false;
            if (object.ReferenceEquals(right, null)) return false;
            if (left.hops.Count != right.hops.Count) return false;
            
            for (int index = 0; index < left.hops.Count; index++)
            {
                if (left.hops[index] != right.hops[index]) return false;
            }

            return true;
        }

        /// <summary>
        /// Compare two AS traceroute paths for inequality.
        /// </summary>
        /// <param name="left">The first path.</param>
        /// <param name="right">The second path.</param>
        /// <returns><b>True</b> if the paths are different, <b>false</b> otherwise.</returns>
        public static bool operator !=(ASTraceroutePath left, ASTraceroutePath right)
        {
            return !(left == right);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds a new hop at the end of the path for a missing AS.
        /// </summary>
        /// <returns>The hop.</returns>
        public ASTracerouteHop AddHop()
        {
            ASTracerouteHop hop = new ASTracerouteHop();
            this.hops.Add(hop);
            return hop;
        }

        /// <summary>
        /// Adds a new hop at the end of the path for the list of specified ASes.
        /// </summary>
        /// <param name="list">The list of ASes.</param>
        public ASTracerouteHop AddHop(List<ASInformation> list)
        {
            ASTracerouteHop hop = new ASTracerouteHop(list);
            this.hops.Add(hop);
            return hop;
        }

        /// <summary>
        /// Adds a source hop.
        /// </summary>
        /// <param name="list">The AS list.</param>
        public void AddSource(List<ASInformation> list)
        {
            this.hops.Insert(0, new ASTracerouteHop(list));
        }

        /// <summary>
        /// Adds a destination hop.
        /// </summary>
        /// <param name="list">The AS list.</param>
        public void AddDestination(List<ASInformation> list)
        {
            this.hops.Add(new ASTracerouteHop(list));
        }

        /// <summary>
        /// Compares this AS traceroute path for equality.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><b>True</b> if the path is equal, <b>false</b> otherwise.</returns>
        public override bool Equals(object path)
        {
            return this == (path as ASTraceroutePath);
        }

        /// <summary>
        /// Gets the hash code for this AS traceroute path.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach (ASTracerouteHop hop in this.hops)
                hashCode ^= hop.GetHashCode();
            return hashCode;
        }

        #endregion
    }
}
