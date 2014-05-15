using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Topology
{
    public enum ASTracerouteFlags
    {
        None = 0,
        // Attempt traceroute
        MultipleAsEqualNeighbor = 0x1, // [AS0] - [AS0 AS1] - [AS?]
        MultipleAsDifferentNeighbor = 0x10000, // [AS0] - [AS1 AS2] - [AS3]
        MultipleEqualNeighborPair = 0x2, // [AS0] - [AS0 AS?] - [AS0 AS?] - [AS?]
        MultipleDifferentNeighborPair = 0x20000, // [AS?] - [AS? AS?] - [AS? AS?] - [AS?]
        MissingHopInsideAs = 0x4,
        MissingHopEdgeAs = 0x40000,
        MissingSource = 0x80000,
        MissingDestination = 0x100000,
        LoopPath = 0x200000,
        // Flow traceroute
        FlowNotEnoughAttempts = 0x400000,
        FlowDistinctAttempts = 0x800000
    }
}
