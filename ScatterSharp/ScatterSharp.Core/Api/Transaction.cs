using System.Collections.Generic;

namespace ScatterSharp.Core.Api
{
    public class Transaction
    {
        public IEnumerable<object> abis;
        public string serializedTransaction;
        public string chainId;
    }
}