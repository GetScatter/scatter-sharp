using EosSharp;
using EosSharp.Core.Api.v1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScatterSharp.Core;
using ScatterSharp.Core.Api;
using ScatterSharp.EosProvider;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;

namespace ScatterSharp.UnitTests
{
    [TestClass]
    public class ScatterEosUnitTests
    {
        //mainnet
        //public static readonly Network network = new Network()
        //{
        //    blockchain = Scatter.Blockchains.EOSIO,
        //    host = "api.eossweden.se",
        //    port = 443,
        //    chainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906"
        //};

        //jungle testnet
        public static readonly Network network = new Network()
        {
            blockchain = ScatterConstants.Blockchains.EOSIO,
            host = "jungle2.cryptolions.io",
            port = 443,
            chainId = "e70aaab8997e1dfce58fbfac80cbbb8fecec7b99cf982a9444273cbc64c41473"
        };

        public Scatter Scatter { get; set; }

        public ScatterEosUnitTests()
        {
            Scatter = new Scatter(new ScatterConfigurator()
            {
                AppName = "MyApp",
                Network = network
            });
        }

        [TestMethod]
        [TestCategory("Scatter EOS Tests")]
        public async Task PushTransaction()
        {
            try
            {
                await Scatter.Connect();

                await Scatter.GetIdentity(new IdentityRequiredFields()
                {
                    accounts = new List<ScatterSharp.Core.Api.Network>()
                    {
                        network
                    },
                    location = new List<LocationFields>(),
                    personal = new List<PersonalFields>()
                });

                var eos = new Eos(new EosSharp.Core.EosConfigurator()
                {
                    ChainId = network.chainId,
                    HttpEndpoint = network.GetHttpEndpoint(),
                    SignProvider = new ScatterSignatureProvider(Scatter)
                });

                var account = Scatter.Identity.accounts.First();

                var result = await eos.CreateTransaction(new EosSharp.Core.Api.v1.Transaction()
                {
                    actions = new List<EosSharp.Core.Api.v1.Action>()
                    {
                        new EosSharp.Core.Api.v1.Action()
                        {
                            account = "eosio.token",
                            authorization =  new List<PermissionLevel>()
                            {
                                new PermissionLevel() { actor = account.name, permission = account.authority }
                            },
                            name = "transfer",
                            data = new Dictionary<string, string>()
                            {
                                { "from", account.name },
                                { "to", "auaglobalts5" },
                                { "quantity", "0.1000 EOS" },
                                { "memo", "" }
                            }
                        }
                }
                });
                Console.WriteLine(JsonConvert.SerializeObject(result));                
            }
            catch(Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
                Assert.Fail();
            }
        }
    }
}
