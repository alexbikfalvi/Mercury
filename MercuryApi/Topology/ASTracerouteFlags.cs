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

namespace Mercury.Topology
{
    /// <summary>
    /// An enumeration with AS traceroute flags.
    /// </summary>
    public enum ASTracerouteFlags
    {
        None = 0,
        // Hop
        MissingAs = 0x1,
        MultipleAs = 0x2,
        // Attempt traceroute
        MultipleAsEqualNeighbor = 0x4, // [AS0] - [AS0 AS1] - [AS?]
        MultipleAsDifferentNeighbor = 0x10000, // [AS0] - [AS1 AS2] - [AS3]
        MultipleAsDifferentNeighborIXP = 0x1000, // [AS0] - [AS1 AS2]<IXP> - [AS3]
        MultipleEqualNeighborPair = 0x8, // [AS0] - [AS0 AS?] - [AS0 AS?] - [AS?]
        MultipleDifferentNeighborPair = 0x20000, // [AS?] - [AS? AS?] - [AS? AS?] - [AS?]
        MissingHopInsideAs = 0x10,
        MissingHopEdgeAs = 0x400, //Temporary change the level (0x40000) for (0x400)
        MissingSource = 0x80000,
        MissingDestination = 0x100000,
        LoopPath = 0x200000,
        TooManyMissingHops = 0x40000,
        // Flow traceroute
        FlowNotEnoughAttempts = 0x400000,
        FlowDistinctAttempts = 0x800000,
    }

    /// <summary>
    /// A class with extension methods for AS traceroute flag.
    /// </summary>
    public static class ASTracerouteFlagsExtensions
    {
        /// <summary>
        /// Checks whether an AS traceroute flag is successful.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <returns><b>True</b> if the flag is successful, <b>false</b> otherwise.</returns>
        public static bool IsSuccessful(this ASTracerouteFlags flag)
        {
            return (int)flag < 0x10000;
        }
    }
}
