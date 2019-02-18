using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class AuthenticateRequest : ApiBase
    {
        public string nonce;
        public string data;
        public string publicKey;
    }
}
