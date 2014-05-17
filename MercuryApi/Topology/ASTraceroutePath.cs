using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercury.Api;

namespace Mercury.Topology
{
    public sealed class ASTraceroutePath
    {
        private readonly List<MercuryAsTracerouteHop> hops = new List<MercuryAsTracerouteHop>();
        private ASTracerouteFlags flag = ASTracerouteFlags.None;

        public void addHop(MercuryAsTracerouteHop asHop){
            this.hops.Add(asHop);
        }

    }
}
