using System;
using System.Collections.Generic;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class Identity
    {
        public string hash;
        public string publicKey;
        public string name;
        public bool kyc;
        public List<IdentityAccount> accounts;
    }
}
