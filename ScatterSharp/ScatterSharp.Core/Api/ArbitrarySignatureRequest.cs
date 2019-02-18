using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class ArbitrarySignatureRequest : ApiBase
    {
        public string publicKey;
        public string data;
        public string whatfor;
        public bool   isHash;
    }
}
