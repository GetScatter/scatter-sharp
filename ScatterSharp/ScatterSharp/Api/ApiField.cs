using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Api
{
    public class ApiField
    {
        [JsonProperty("blockchain")]
        public string Blockchain { get; set; }
        [JsonProperty("host")]
        public string Host { get; set; }
        [JsonProperty("port")]
        public string Port { get; set; }
        [JsonProperty("chainId")]
        public string ChainId { get; set; }
    }
}
