using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Api
{
    public class Ip2AsnMapping
    {
        [JsonProperty("as")]
        public int As{ get; set; }
        //[JsonProperty("asName")]
	    public String AsName{ get; set; }
	    [JsonProperty("rangeLow")]
        public long RangeLow{ get; set; }
        [JsonProperty("rangeHigh")]
	    public long RangeHigh{ get; set; }
        [JsonProperty("numIps")]
	    public long NumIps{ get; set; }
        [JsonProperty("prefix")]
	    public String Prefix{ get; set; }
        [JsonProperty("ixpParticipant")]
	    public int IxpParticipant{ get; set; }
        [JsonProperty("ixpParticipantName")]
	    public String IxpParticipantName{ get; set; }
        //[JsonProperty("timeStamp")]
        //public DateTime TimeStamp { get; set; }
        [JsonProperty("type")]
	    public String Type{ get; set; }
        [JsonProperty("ip")]
	    public String Ip{ get; set; }
	    //public Location location{ get; set; }
    }
}
