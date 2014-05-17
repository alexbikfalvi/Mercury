using Mercury.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Topology
{
    public class ASTraceroutePath
    {
        public List<ASTracerouteHop> hops = new List<ASTracerouteHop>(); 
        public Dictionary<int, MercuryAsTracerouteRelationship> relationships = new Dictionary<int, MercuryAsTracerouteRelationship>();

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


    }
}
