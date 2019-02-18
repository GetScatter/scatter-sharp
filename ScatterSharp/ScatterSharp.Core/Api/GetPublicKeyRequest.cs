using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class GetPublicKeyRequest : ApiBase
    {
        public string blockchain;
    }
}
