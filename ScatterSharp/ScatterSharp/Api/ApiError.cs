using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Api
{
    public class ApiError
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("isError")]
        public string IsError { get; set; }
    }
}
