using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Api
{
    public class TracerouteSettings
    {
        public String id{ get; set; }
        public int attemptsPerFlow{ get; set; }
        public int flowCount{ get; set; }
        public int minHops{ get; set; }
        public int maxHops{ get; set; }
        public int attemptDelay{ get; set; }
        public int hopTimeout{ get; set; }
        public int minPort{ get; set; }
        public int maxPort{ get; set; }
        public int dataLength{ get; set; }

        public TracerouteSettings(){}

        public TracerouteSettings(String id, int attemptsPerFlow, int flowCount, int minHops, int maxHops, int attemptDelay, int hopTimeout, int minPort, int maxPort, int dataLength)
        {
            this.id = id;
            this.attemptsPerFlow = attemptsPerFlow;
            this.flowCount = flowCount;
            this.minHops = minHops;
            this.maxHops = maxHops;
            this.attemptDelay = attemptDelay;
            this.hopTimeout = hopTimeout;
            this.minPort = minPort;
            this.maxPort = maxPort;
            this.dataLength = dataLength;

        }
    }
}
