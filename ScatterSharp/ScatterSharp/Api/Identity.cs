using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Api
{
    public class Identity
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }
        [JsonProperty("publicKey")]
        public string PublicKey { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("kyc")]
        public bool Kyc { get; set; }
        [JsonProperty("accounts")]
        public List<IdentityAccount> Accounts { get; set; }
    }
}
