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
            this.hops.Add(asHops);
        }

    }
}
