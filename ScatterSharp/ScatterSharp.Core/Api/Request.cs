using System;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class Request<T>
    {
        public string id;
        public string appkey;
        public string nonce;
        public string nextNonce;

        public string type;
        public T payload;
    }
}
