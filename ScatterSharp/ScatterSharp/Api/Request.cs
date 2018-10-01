using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Api
{
    public class Request
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("appkey")]
        public string Appkey { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        [JsonProperty("nextnonce")]
        public string NextNonce { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("payload")]
        public object Payload { get; set; }
    }
}
