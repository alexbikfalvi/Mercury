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

        public void addHopsAtBegining(List<MercuryAsTracerouteHop> asHops)
        {
            Dictionary<int, MercuryAsTracerouteHop> dictionary = new Dictionary<int, MercuryAsTracerouteHop>();
            //First we remove duplicate multiple ases
            foreach (MercuryAsTracerouteHop hop in asHops)
            {
                dictionary[hop.AsNumber] = hop;
            }
            Hop h = new Hop();
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
            Hop h = new Hop();
            h.candidates = dictionary;
            this.hops.Add(h);
        }


    }



    public class Hop
    {
        //key=ASnumber, value=MercuryAsTracerouteHop
        public Dictionary<int, MercuryAsTracerouteHop> candidates = new Dictionary<int, MercuryAsTracerouteHop>();


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


        public bool isEqualUnique(Hop hop2)
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

        public MercuryAsTracerouteHop getEqualUnique(Hop hop2)
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


        public bool isEqualUniqueToMultiple(Hop hop2) 
        {
            if ( (candidates.Count == 1 && hop2.candidates.Count > 1) || (candidates.Count > 1 && hop2.candidates.Count == 1) )
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

        public MercuryAsTracerouteHop getEqualUniqueToMultiple(Hop hop2)
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

        public bool isEqualMultipleToMultiple(Hop hop2) 
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
                else {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //We might return just one AS, instead of all the list again...
        public MercuryAsTracerouteHop getEqualMultipleToMultiple(Hop hop2)
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
