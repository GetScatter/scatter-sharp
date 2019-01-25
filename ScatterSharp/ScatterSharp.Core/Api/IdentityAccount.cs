using System;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class IdentityAccount
    {
        public string name;
        public string authority;
        public string publicKey;
        public string blockchain;
        public bool   isHardware;
        public string chainId;
    }
}
