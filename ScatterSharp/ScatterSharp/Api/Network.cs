using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Api
{
    public class Network
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("blockchain")]
        public string Blockchain { get; set; }
        [JsonProperty("host")]
        public string Host { get; set; }
        [JsonProperty("port")]
        public int? Port { get; set; }
        [JsonProperty("chainId")]
        public string ChainId { get; set; }
    }
}