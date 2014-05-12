using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Api
{
    public class TracerouteAS
    {
        //[JsonProperty("srcAS")]
        public int srcAS { get; set; }
        [JsonProperty("srcASName")]
        public String srcASName { get; set; }
        [JsonProperty("srcIp")]
        public String srcIp { get; set; }
        [JsonProperty("srcPublicIp")]
        public String srcPublicIp { get; set; }
        [JsonProperty("srcCity")]
        public String srcCity { get; set; }
        [JsonProperty("srcCountry")]
        public String srcCountry { get; set; }
        [JsonProperty("dstAS")]
        public int dstAS { get; set; }
        [JsonProperty("dstASName")]
        public String dstASName { get; set; }
        [JsonProperty("dstIp")]
        public String dstIp { get; set; }
        [JsonProperty("dst")]
        public String dst { get; set; }
        [JsonProperty("dstCity")]
        public String dstCity { get; set; }
        [JsonProperty("dstCountry")]
        public String dstCountry { get; set; }
        [JsonProperty("timeStamp")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime timeStamp { get; set; }
        [JsonProperty("tracerouteIpAttemptIds")]
        public List<String> tracerouteIpAttemptIds { get; set; }
        [JsonProperty("tracerouteASHops")]
        public List<TracerouteASHop> tracerouteASHops { get; set; }
        [JsonProperty("tracerouteASRelationships")]
        public List<TracerouteASRelationship> tracerouteASRelationships { get; set; }
        [JsonProperty("tracerouteASStats")]
        public TracerouteASStats tracerouteASStats { get; set; }


        public TracerouteAS()
        {
            this.tracerouteIpAttemptIds = new List<string>();
            this.tracerouteASHops = new List<TracerouteASHop>();
            this.tracerouteASRelationships = new List<TracerouteASRelationship>();
        }
        public TracerouteAS(int srcAS, String srcASName, String srcIp,
                    String srcPublicIp, String srcCity, String srcCountry, int dstAS,
                    String dstASName, String dstIp, String dst, String dstCity,
                    String dstCountry, DateTime timeStamp, TracerouteASStats tracerouteASStats)
        {
            this.srcAS = srcAS;
            this.srcASName = srcASName;
            this.srcIp = srcIp;
            this.srcPublicIp = srcPublicIp;
            this.srcCity = srcCity;
            this.srcCountry = srcCountry;
            this.dstAS = dstAS;
            this.dstASName = dstASName;
            this.dstIp = dstIp;
            this.dst = dst;
            this.dstCity = dstCity;
            this.dstCountry = dstCountry;
            this.timeStamp = timeStamp;
            this.tracerouteASStats = tracerouteASStats;

            this.tracerouteIpAttemptIds = new List<string>();
            this.tracerouteASHops = new List<TracerouteASHop>();
            this.tracerouteASRelationships = new List<TracerouteASRelationship>();
        }
    }

    public class TracerouteASStats
    {
        public int asHops { get; set; }
        public int c2pRels { get; set; }
        public int p2pRels { get; set; }
        public int p2cRels { get; set; }
        public int s2sRels { get; set; }
        public int ixpRels { get; set; }
        public int nfRels { get; set; }
        public bool completed { get; set; }
        public int flags { get; set; }


        public TracerouteASStats() { }

        public TracerouteASStats(int asHops, int c2pRels, int p2pRels, int p2cRels,
                int s2sRels, int ixpRels, int nfRels, bool completed,
                int flags)
        {
            this.asHops = asHops;
            this.c2pRels = c2pRels;
            this.p2pRels = p2pRels;
            this.p2cRels = p2cRels;
            this.s2sRels = s2sRels;
            this.ixpRels = ixpRels;
            this.nfRels = nfRels;
            this.completed = completed;
            this.flags = flags;
        }

    }


    public class TracerouteASRelationship
    {
        public enum Relationship { C2P, P2P, P2C, S2S, IXP, NF };

        public Relationship relationship { get; set; }
        public int as0 { get; set; }
        public int as1 { get; set; }
        public int hop { get; set; }

        public TracerouteASRelationship() { }

        public TracerouteASRelationship(Relationship relationship, int as0, int as1, int hop)
        {
            this.relationship = relationship;
            this.as0 = as0;
            this.as1 = as1;
            this.hop = hop;
        }

    }

    public class TracerouteASHop
    {
        public enum Type { AS, IXP };

        public int hop;
        [JsonProperty("as")]
        public int asNumber;
        public String asName;
        public String ixpName;
        public Type type;
        public bool inferred;//We use an heuristic to determine what is the most suitable AS

        public TracerouteASHop() { }

        public TracerouteASHop(int hop, int asNumber, String asName, String ixpName, Type type, bool inferred)
        {
            this.hop = hop;
            this.asNumber = asNumber;
            this.asName = asName;
            this.ixpName = ixpName;
            this.type = type;
            this.inferred = inferred;
        }
    }
}
