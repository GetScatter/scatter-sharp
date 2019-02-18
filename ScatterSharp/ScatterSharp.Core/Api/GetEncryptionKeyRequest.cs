using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class GetEncryptionKeyRequest : ApiBase
    {
        public string fromPublicKey;
        public string toPublicKey;
        public UInt64 nonce;
    }
}
