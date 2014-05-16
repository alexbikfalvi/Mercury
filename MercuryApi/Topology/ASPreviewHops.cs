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
        public List<List<TracerouteASHop>> hops = new List<List<TracerouteASHop>>();
        public Dictionary<int, MercuryAsTracerouteRelationship> relationships = new Dictionary<int, MercuryAsTracerouteRelationship>();

        public ASTracerouteFlags flags = ASTracerouteFlags.None;

        public void addHops(List<TracerouteASHop> asHops)
        {
            Dictionary<int, TracerouteASHop> dictionary = new Dictionary<int, TracerouteASHop>();
            //First we remove duplicate multiple ases
            foreach (TracerouteASHop hop in asHops)
            {
                dictionary[hop.asNumber] = hop;
            }
            this.hops.Add(new List<TracerouteASHop>(dictionary.Values));
        }

        public void addHopsAtBegining(List<TracerouteASHop> asHops)
        {
            Dictionary<int, TracerouteASHop> dictionary = new Dictionary<int, TracerouteASHop>();
            //First we remove duplicate multiple ases
            foreach (TracerouteASHop hop in asHops)
            {
                dictionary[hop.asNumber] = hop;
            }
            this.hops.Insert(0, new List<TracerouteASHop>(dictionary.Values));
        }

        public void addHopsAtEnd(List<TracerouteASHop> asHops)
        {
            Dictionary<int, TracerouteASHop> dictionary = new Dictionary<int, TracerouteASHop>();
            //First we remove duplicate multiple ases
            foreach (TracerouteASHop hop in asHops)
            {
                dictionary[hop.asNumber] = hop;
            }
            this.hops.Add(new List<TracerouteASHop>(dictionary.Values));
        }

    }
}
