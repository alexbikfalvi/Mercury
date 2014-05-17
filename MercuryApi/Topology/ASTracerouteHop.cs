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
    /// A class for a AS traceroute hop.
    /// </summary>
    public class ASTracerouteHop
    {
        //key=ASnumber, value=MercuryAsTracerouteHop
        public Dictionary<int, MercuryAsTracerouteHop> candidates = new Dictionary<int, MercuryAsTracerouteHop>();


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
    }
}
