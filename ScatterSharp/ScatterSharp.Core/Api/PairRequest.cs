using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class PairRequest : ApiBase
    {
        public string appkey;
        public bool passthrough;
    }
}
