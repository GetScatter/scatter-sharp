using System;
using System.Collections.Generic;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class SignatureRequest : ApiBase
    {
        public Network network;
        public string blockchain;
        public List<object> requiredFields;
        public Transaction transaction;
    }
}
