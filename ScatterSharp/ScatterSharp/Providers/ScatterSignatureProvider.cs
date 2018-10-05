using EosSharp;
using EosSharp.Api.v1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

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
            var identity = await Scatter.GetIdentity(new Api.IdentityRequiredFields()
            {
                Accounts = new List<Api.Network>()
                {
                    Scatter.Network
                },
                Location = new List<Api.LocationFields>(),
                Personal = new List<Api.PersonalFields>()
            });

            if (identity == null)
                throw new ArgumentNullException("identity");

            if (identity.Accounts == null)
                throw new ArgumentNullException("identity.Accounts");

            return identity.Accounts.Select(acc => acc.PublicKey);
        }

        public async Task<IEnumerable<string>> Sign(string chainId, List<string> requiredKeys, byte[] signBytes = null, Transaction trx = null)
        {
            var result = await Scatter.RequestSignature(new
            {
                network = Scatter.Network,
                blockchain = Scatter.Blockchains.EOSIO,
                requiredFields = new List<object>(),
                transaction = trx,
                origin = Scatter.AppName
            });

            return new List<string>();
        }
    }
}
