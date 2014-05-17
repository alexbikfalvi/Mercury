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
        public readonly List<ASTracerouteHop> hops = new List<ASTracerouteHop>();

        #region Public properties

        /// <summary>
        /// Gets the flags for this AS traceroute path.
        /// </summary>
        public ASTracerouteFlags Flags { get; private set; }
        /// <summary>
        /// Returns whether this AS path is successful.
        /// </summary>
        public bool IsSuccessful { get { return this.Flags.IsSuccessful(); } }

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

        public void addHopsAtBegining(List<MercuryAsTracerouteHop> asHops)
        {
            Dictionary<int, MercuryAsTracerouteHop> dictionary = new Dictionary<int, MercuryAsTracerouteHop>();
            //First we remove duplicate multiple ases
            foreach (MercuryAsTracerouteHop hop in asHops)
            {
                dictionary[hop.AsNumber] = hop;
            }
            ASTracerouteHop h = new ASTracerouteHop();
            h.candidates = dictionary;
            this.hops.Insert(0, h);
        }

        public void addHopsAtEnd(List<MercuryAsTracerouteHop> asHops)
        {
            Dictionary<int, MercuryAsTracerouteHop> dictionary = new Dictionary<int, MercuryAsTracerouteHop>();
            //First we remove duplicate multiple ases
            foreach (MercuryAsTracerouteHop hop in asHops)
            {
                dictionary[hop.AsNumber] = hop;
            }
            ASTracerouteHop h = new ASTracerouteHop();
            h.candidates = dictionary;
            this.hops.Add(h);
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
