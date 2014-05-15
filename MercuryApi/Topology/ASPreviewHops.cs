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
        private ASTracerouteFlags flag = ASTracerouteFlags.None;

        public void addHops(List<TracerouteASHop> asHops)
        {

            Dictionary<int, TracerouteASHop> dictionary = new Dictionary<int, TracerouteASHop>();
            //First we remove duplicate multiple ases
            foreach (TracerouteASHop hop in asHops)
            {
                dictionary[hop.asNumber] = hop;
                //try { 

                //    dictionary.Add(hop.asNumber, hop);
                //}
                //catch (ArgumentException) { }
            }

            this.hops.Add(new List<TracerouteASHop>(dictionary.Values));
            //this.hops.Add(asHops);
        }

    }
}
