using Mercury.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Mercury.Topology
{
    public class ASPreviewHops
    {
        public List<List<MercuryAsTracerouteHop>> hops = new List<List<MercuryAsTracerouteHop>>();
        public Dictionary<int, MercuryAsTracerouteRelationship> relationships = new Dictionary<int, MercuryAsTracerouteRelationship>();

        public ASTracerouteFlags flags = ASTracerouteFlags.None;

        public void addHops(List<MercuryAsTracerouteHop> asHops)
        {
            Dictionary<int, MercuryAsTracerouteHop> dictionary = new Dictionary<int, MercuryAsTracerouteHop>();
            //First we remove duplicate multiple ases
            foreach (MercuryAsTracerouteHop hop in asHops)
            {
                dictionary[hop.AsNumber] = hop;
            }
            this.hops.Add(new List<MercuryAsTracerouteHop>(dictionary.Values));
        }

        public void addHopsAtBegining(List<MercuryAsTracerouteHop> asHops)
        {
            Dictionary<int, MercuryAsTracerouteHop> dictionary = new Dictionary<int, MercuryAsTracerouteHop>();
            //First we remove duplicate multiple ases
            foreach (MercuryAsTracerouteHop hop in asHops)
            {
                dictionary[hop.AsNumber] = hop;
            }
            this.hops.Insert(0, new List<MercuryAsTracerouteHop>(dictionary.Values));
        }

        public void addHopsAtEnd(List<MercuryAsTracerouteHop> asHops)
        {
            Dictionary<int, MercuryAsTracerouteHop> dictionary = new Dictionary<int, MercuryAsTracerouteHop>();
            //First we remove duplicate multiple ases
            foreach (MercuryAsTracerouteHop hop in asHops)
            {
                dictionary[hop.AsNumber] = hop;
            }
            this.hops.Add(new List<MercuryAsTracerouteHop>(dictionary.Values));
        }

    }
}
