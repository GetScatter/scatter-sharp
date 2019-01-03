using EosSharp.Api.v1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScatterSharp.UnitTests
{
    [TestClass]
    public class ScatterEosUnitTests
    {
        //mainnet
        public static readonly Core.Api.Network network = new Core.Api.Network()
        {
            blockchain = Scatter.Blockchains.EOSIO,
            host = "api.eossweden.se",
            port = 443,
            chainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906"
        };

        //btuga testnet
        //public static readonly Api.Network network = new Api.Network()
        //{
        //    Blockchain = Scatter.Blockchains.EOSIO,
        //    Host = "nodeos01.btuga.io",
        //    Port = 443,
        //    ChainId = "cf057bbfb72640471fd910bcb67639c22df9f92470936cddc1ade0e2f2e7dc4f"
        //};

        public Scatter Scatter { get; set; }

        public ScatterEosUnitTests()
        {
            Scatter = new Scatter("SCATTER-SHARP-EOS", network);
        }

        [TestMethod]
        [TestCategory("Scatter EOS Tests")]
        public async Task Connect()
        {
            await Scatter.Connect();
            var eos = Scatter.Eos();
        }

        [TestMethod]
        [TestCategory("Scatter EOS Tests")]
        public async Task PushTransaction()
        {            
            bool success = false;
            try
            {
                await Scatter.Connect();

                var identity = await Scatter.GetIdentity(new Core.Api.IdentityRequiredFields()
                {
                    accounts = new List<Core.Api.Network>()
                    {
                        Scatter.Network
                    },
                    location = new List<Core.Api.LocationFields>(),
                    personal = new List<Core.Api.PersonalFields>()
                });

                var eos = Scatter.Eos();

                var result = await eos.CreateTransaction(new Transaction()
                {
                    Actions = new List<EosSharp.Api.v1.Action>()
                    {
                        new EosSharp.Api.v1.Action()
                        {
                            Account = "eosio.token",
                            Authorization = new List<PermissionLevel>()
                            {
                                new PermissionLevel() {Actor = "tester112345", Permission = "active" }
                            },
                            Name = "transfer",
                            Data = new { from = "tester112345", to = "tester212345", quantity = "0.0001 EOS", memo = "hello crypto world!" }
                        }
                    }
                });
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            Assert.IsTrue(success);
        }
    }
}
