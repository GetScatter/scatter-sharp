using EosSharp;
using EosSharp.Api.v1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScatterSharp.Providers
{
    public class ScatterSignatureProvider : ISignProvider
    {
        private Scatter Scatter { get; set; }

        public ScatterSignatureProvider(Scatter scatter)
        {
            Scatter = scatter;
        }

        public Task<IEnumerable<string>> GetAvailableKeys()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Sign(string chainId, List<string> requiredKeys, byte[] signBytes = null, Transaction trx = null)
        {
            throw new NotImplementedException();
        }
    }
}
