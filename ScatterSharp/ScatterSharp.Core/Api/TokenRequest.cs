using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class TokenRequest : ApiBase
    {
        public Token token;
        public Network network;
    }
}
