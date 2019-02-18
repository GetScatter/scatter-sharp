using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class LinkAccountRequest : ApiBase
    {
        public LinkAccount account;
        public Network network;
    }
}
