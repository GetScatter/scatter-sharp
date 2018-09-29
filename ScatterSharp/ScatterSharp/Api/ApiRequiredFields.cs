using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Api
{
    public class ApiRequiredFields
    {
        [JsonProperty("accounts")]
        public List<ApiField> Accounts { get; set; }
    }
}
