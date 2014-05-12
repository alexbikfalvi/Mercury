using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Api
{
    public class Ip2GeoMapping
    {
        [JsonProperty("ip")]
        public String Ip { get; set; }
        [JsonProperty("countryCode")]
        public String CountryCode { get; set; }
        [JsonProperty("countryName")]
        public String CountryName { get; set; }
        [JsonProperty("city")]
        public String City { get; set; }
    }
}
