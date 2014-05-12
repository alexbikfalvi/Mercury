using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Api
{
    public class MyInfo
    {

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("as")]
        public int As { get; set; }

        //[JsonProperty("asName")]
        public string ASName { get; set; }

        //[JsonProperty("timeStamp")]
        //public DateTime TimeStamp { get; set; }
    }
}
