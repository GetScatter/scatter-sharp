using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class TransferRequest : ApiBase
    {
        public Network network;
        public string to;
        public string amount;
        public object options;
    }
}
