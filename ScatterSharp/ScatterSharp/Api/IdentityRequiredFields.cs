using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Api
{
    public class IdentityRequiredFields
    {
        [JsonProperty("accounts")]
        public List<Network> Accounts { get; set; }
        [JsonProperty("personal")]
        public List<PersonalFields> Personal { get; set; }
        [JsonProperty("location")]
        public List<LocationFields> Location { get; set; }
    }
}
