using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Api
{
    public class SignaturesResult
    {
        [JsonProperty("signatures")]
        public List<string> Signatures { get; set; }
        [JsonProperty("returnedFields")]
        public IdentityRequiredFields ReturnedFields { get; set; }
    }
}
