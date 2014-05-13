using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercury.Api;

namespace Mercury.Topology
{
    public sealed class ASTraceroutePath
    {
        private readonly List<TracerouteASHop> hops = new List<TracerouteASHop>();
        private uint flag = 0;
    }
}
