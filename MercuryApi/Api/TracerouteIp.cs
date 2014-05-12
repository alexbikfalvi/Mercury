using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Api
{
    public class TracerouteIp
    {
        public String srcIp { get; set; }
        public String srcPublicIp { get; set; }
        public String dstIp { get; set; }
        public String dst { get; set; }
        public List<TracerouteIpFlow> tracerouteIpFlows { get; set; }
        public String tracerouteSettingsId { get; set; }
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime timeStamp { get; set; }

        public TracerouteIp()
        {
            this.tracerouteIpFlows = new List<TracerouteIpFlow>();
        }
        public TracerouteIp(String srcIp, String srcPublicIp, String dstIp, String dst, String tracerouteSettingsId, DateTime timeStamp)
        {
            this.srcIp = srcIp;
            this.srcPublicIp = srcPublicIp;
            this.dstIp = dstIp;
            this.dst = dst;
            this.tracerouteSettingsId = tracerouteSettingsId;
            this.tracerouteIpFlows = new List<TracerouteIpFlow>();
            this.timeStamp = timeStamp;
        }
    }

    public class TracerouteIpFlow
    {
        public enum Algorithm { ICMP, UDP_IDENTIFICATION, UDP_CHECKSUM, UDP_BOTH }

        public List<TracerouteIpAttempt> tracerouteIpAttemps { get; set; }
        public Algorithm algorithm { get; set; }
        public String flowId { get; set; }

        public TracerouteIpFlow()
        {
            this.tracerouteIpAttemps = new List<TracerouteIpAttempt>();
        }

        public TracerouteIpFlow(Algorithm algorithm, String flowId)
        {
            this.algorithm = algorithm;
            this.flowId = flowId;
            this.tracerouteIpAttemps = new List<TracerouteIpAttempt>();
        }
    }

    public class TracerouteIpAttempt
    {
        public enum State { COMPLETED, UNREACHABLE }

        public List<TracerouteIpHop> tracerouteIpHops { get; set; }
        public TracerouteIpAttempt.State state { get; set; }
        public int maxTTL { get; set; }
        public String tracerouteIpAttemptId { get; set; } //This ID must be manually generated and must match 
        //with the list of IDs of TracerouteAS.tracerouteIpAttemptIds String UUID = Guid.NewGuid().ToString();


        public TracerouteIpAttempt()
        {
            this.tracerouteIpHops = new List<TracerouteIpHop>();
        }

        public TracerouteIpAttempt(TracerouteIpAttempt.State state, int maxTTL, String tracerouteIpAttemptId)
        {
            this.state = state;
            this.maxTTL = maxTTL;
            this.tracerouteIpAttemptId = tracerouteIpAttemptId;
            this.tracerouteIpHops = new List<TracerouteIpHop>();
        }
    }

    public class TracerouteIpHop
    {
        public enum State { REQ_SENT, RESP_RECV }
        public enum Type { ECHO_REPLY, TIME_EXCEEDED, DESTINATION_UNREACHABLE, UNKNOWN }

        public TracerouteIpHop.State state { get; set; }
        public TracerouteIpHop.Type type { get; set; }
        public String ipAddr { get; set; }
        public int ttl { get; set; }
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime reqTimeStamp{ get; set; }
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime respTimeStamp{ get; set; }

        public TracerouteIpHop() { }

        public TracerouteIpHop(TracerouteIpHop.State state, TracerouteIpHop.Type type, String ipAddr, int ttl)
        {
            this.state = state;
            this.type = type;
            this.ipAddr = ipAddr;
            this.ttl = ttl;
        }


    }
}
