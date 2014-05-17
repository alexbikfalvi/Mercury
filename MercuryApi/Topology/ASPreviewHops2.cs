using Mercury.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Topology
{
    public class ASPreviewHops2
    {
        public List<Hop> hops = new List<Hop>(); 
        public Dictionary<int, MercuryAsTracerouteRelationship> relationships = new Dictionary<int, MercuryAsTracerouteRelationship>();

        public void addHopsAtBegining(List<TracerouteASHop> asHops)
        {
            Dictionary<int, TracerouteASHop> dictionary = new Dictionary<int, TracerouteASHop>();
            //First we remove duplicate multiple ases
            foreach (TracerouteASHop hop in asHops)
            {
                dictionary[hop.asNumber] = hop;
            }
            Hop h = new Hop();
            h.candidates = dictionary;
            this.hops.Insert(0, h);
        }

        public void addHopsAtEnd(List<TracerouteASHop> asHops)
        {
            Dictionary<int, TracerouteASHop> dictionary = new Dictionary<int, TracerouteASHop>();
            //First we remove duplicate multiple ases
            foreach (TracerouteASHop hop in asHops)
            {
                dictionary[hop.asNumber] = hop;
            }
            Hop h = new Hop();
            h.candidates = dictionary;
            this.hops.Add(h);
        }


    }



    public class Hop
    {
        //key=ASnumber, value=TracerouteASHop
        public Dictionary<int, TracerouteASHop> candidates = new Dictionary<int, TracerouteASHop>();


        public bool isMissing(Hop hop2)
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

        public bool isMissingInMiddleSameAS(Hop hop2, Hop hop3)
        {
            if (candidates.Count == 0 && hop2.candidates.Count > 0 & hop3.candidates.Count > 0)
            {
                int matchings = 0;
                foreach (TracerouteASHop hop in hop3.candidates.Values)
                {
                        if (hop2.candidates.ContainsKey(hop.asNumber))
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


        public bool isEqualUnique(Hop hop2)
        {
            if (candidates.Count == 1 && hop2.candidates.Count == 1)
            {
                if (candidates[0].asNumber == hop2.candidates[0].asNumber)
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

        public TracerouteASHop getEqualUnique(Hop hop2)
        {
            if (candidates.Count == 1 && hop2.candidates.Count == 1)
            {
                foreach (TracerouteASHop hop in candidates.Values)
                {

                        if (hop2.candidates.ContainsKey(hop.asNumber))
                        {
                            return hop2.candidates[hop.asNumber];
                        }
                }

            }
            return null;
        }


        public bool isEqualUniqueToMultiple(Hop hop2) 
        {
            if ( (candidates.Count == 1 && hop2.candidates.Count > 1) || (candidates.Count > 1 && hop2.candidates.Count == 1) )
            {
                foreach (TracerouteASHop hop in candidates.Values)
                {
                    if (hop2.candidates.ContainsKey(hop.asNumber))
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

        public TracerouteASHop getEqualUniqueToMultiple(Hop hop2)
        {
            if ((candidates.Count == 1 && hop2.candidates.Count > 1) || (candidates.Count > 1 && hop2.candidates.Count == 1))
            {
                foreach (TracerouteASHop hop in candidates.Values)
                {

                        if (hop2.candidates.ContainsKey(hop.asNumber))
                        {
                            return hop2.candidates[hop.asNumber];
                        }

                }

            }
            return null;
        }

        public bool isEqualMultipleToMultiple(Hop hop2) 
        {
            if (candidates.Count > 1 && hop2.candidates.Count > 1)
            {
                int matchings = 0;
                foreach (TracerouteASHop hop in candidates.Values)
                {
                    if (hop2.candidates.ContainsKey(hop.asNumber))
                    {
                       matchings++;
                    }
                }
                if (matchings > 1)
                {
                    return true;
                }
                else {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public Dictionary<int, TracerouteASHop> getEqualMultipleToMultiple(Hop hop2)
        {
            Dictionary<int, TracerouteASHop> cands = new Dictionary<int, TracerouteASHop>();

            if (candidates.Count > 1 && hop2.candidates.Count > 1)
            {
                int matchings = 0;
                foreach (TracerouteASHop hop in candidates.Values)
                {

                        if (hop2.candidates.ContainsKey(hop.asNumber))
                        {
                            matchings++;
                            cands[hop.asNumber] = hop2.candidates[hop.asNumber];
                        }

                }
                if (matchings > 1)
                {
                    return cands;
                }
            }
            return null;
        }
    }
}
