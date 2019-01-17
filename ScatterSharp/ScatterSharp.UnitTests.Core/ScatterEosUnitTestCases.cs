using EosSharp.Core.Api.v1;
using Newtonsoft.Json;
using ScatterSharp.Core.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScatterSharp.UnitTests.Core
{
    public class ScatterEosUnitTestCases
    {
        private Scatter Scatter { get; set; }
        private Network Network { get; set; }

        public ScatterEosUnitTestCases(Scatter scatter, Network network)
        {
            Scatter = scatter;
            Network = network;
        }

        public async Task Connect()
        {
            await Scatter.Connect();
            var eos = Scatter.Eos();
        }

        public async Task<bool> PushTransaction()
        {
            bool success = false;
            try
            {
                await Scatter.Connect();

                var identity = await Scatter.GetIdentity(new IdentityRequiredFields()
                {
                    accounts = new List<Network>()
                    {
                        Scatter.Network
                    },
                    location = new List<LocationFields>(),
                    personal = new List<PersonalFields>()
                });

                var eos = Scatter.Eos();

                var result = await eos.CreateTransaction(new Transaction()
                {
                    actions = new List<EosSharp.Core.Api.v1.Action>()
                    {
                        new EosSharp.Core.Api.v1.Action()
                        {
                            account = "eosio.token",
                            authorization = new List<PermissionLevel>()
                            {
                                new PermissionLevel() {actor = "tester112345", permission = "active" }
                            },
                            name = "transfer",
                            data = new Dictionary<string, object>() {
                                { "from", "tester112345" },
                                { "to", "tester212345" },
                                { "quantity", "0.0001 EOS" },
                                { "memo", "hello crypto world!" }
                            }
                        }
                    }
                });
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            return success;
        }
    }
}
