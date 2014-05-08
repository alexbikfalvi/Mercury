using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Api
{
    public class ASRelationship
    {
        [JsonProperty("as0")]
        public int As0 { get; set; }
        [JsonProperty("as1")]
        public int As1 { get; set; }
        [JsonProperty("relationship")]
        public int Relationship { get; set; } //-1 customer, 0 peer, 1 provider, 2 sibling,  3 ixp, 10 not found
    }
}
