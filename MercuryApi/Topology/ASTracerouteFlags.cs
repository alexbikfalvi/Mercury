using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Topology
{
    public enum ASTracerouteFlags
    {
        // Attempt traceroute
        MultipleAsEqualNeighbor = 0x1,
        MultipleAsDifferentNeighbor = 0x10000,
        MissingHopInsideAs = 0x2,
        MissingHopEdgeAs = 0x20000,
        MissingSource = 0x40000,
        MissingDestination = 0x80000,
        LoopPath = 0x100000,
        // Flow traceroute
        FlowNotEnoughAttempts = 0x200000,
        FlowDistinctAttempts = 0x400000
    }
}
