using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Api
{
    public class IdentityAccount
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("authority")]
        public string Authority { get; set; }
        [JsonProperty("publicKey")]
        public string PublicKey { get; set; }
        [JsonProperty("blockchain")]
        public string Blockchain { get; set; }     
    }
}
