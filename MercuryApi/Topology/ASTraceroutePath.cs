﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercury.Api;

namespace Mercury.Topology
{
    public sealed class ASTraceroutePath
    {
        private readonly List<TracerouteASHop> hops = new List<TracerouteASHop>();
        private ASTracerouteFlags flag = ASTracerouteFlags.None;

        public void addHop(TracerouteASHop asHop){
            this.hops.Add(asHop);
        }

    }
}
