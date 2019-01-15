using EosSharp;
using EosSharp.Core.Interfaces;
using ScatterSharp.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<string>> GetAvailableKeys()
        {
            var identity = await Scatter.GetIdentityFromPermissions();

            if (identity == null)
                throw new ArgumentNullException("identity");

            if (identity.accounts == null)
                throw new ArgumentNullException("identity.Accounts");

            return identity.accounts.Select(acc => acc.publicKey);
        }

        public async Task<IEnumerable<string>> Sign(string chainId, IEnumerable<string> requiredKeys, byte[] signBytes, IEnumerable<string> abiNames = null)
        {
            IEnumerable<object> abis = null;

            if (abiNames != null)
                abis = abiNames.Select(a => new { account_name = a });
            else
                abis = new List<object>();

            var result = await Scatter.RequestSignature(new
            {
                network = Scatter.Network,
                blockchain = Scatter.Blockchains.EOSIO,
                requiredFields = new List<object>(),
                transaction = new
                {
                    abis,
                    serializedTransaction = UtilsHelper.ByteArrayToHexString(signBytes),
                    chainId
                },
                origin = Scatter.AppName
            });

            return result.signatures;
        }
    }
}
