using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using ScatterSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScatterSharp.UnitTests
{
    [TestClass]
    public class ScatterUnitTests
    {
        public static readonly string SCATTER_DESKTOP_WS_HOST = "127.0.0.1:50005";

        //mainnet
        public static readonly Api.Network network = new Api.Network()
        {
            Blockchain = Scatter.Blockchains.EOSIO,
            Host = "nodes.eos42.io",
            Port = 443,
            ChainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906"
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

        public ScatterUnitTests()
        {
            Scatter = new Scatter("SCATTER-SHARP");
        }

        [TestMethod]
        public async Task Connect()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
        }

        [TestMethod]
        public async Task GetVersion()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.GetVersion());
        }

        [TestMethod]
        public async Task GetIdentity()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(JsonConvert.SerializeObject(await GetIdentityFromScatter()));
        }

        [TestMethod]
        public async Task GetIdentityFromPermissions()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.GetIdentityFromPermissions());
        }

        [TestMethod]
        public async Task ForgetIdentity()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.ForgetIdentity());
        }

        [TestMethod]
        public async Task Authenticate()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);

            var identity = await GetIdentityFromScatter();

            Console.WriteLine(await Scatter.Authenticate(UtilsHelper.RandomNumber()));
        }

        [TestMethod]
        public async Task GetArbitrarySignature()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);

            var identity = await GetIdentityFromScatter();

            Console.WriteLine(await Scatter.GetArbitrarySignature(identity.PublicKey, "HELLO WORLD!"));
        }

        [TestMethod]
        public async Task GetPublicKey()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.GetPublicKey(Scatter.Blockchains.EOSIO));
        }

        [TestMethod]
        public async Task LinkAccount()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);

            var pubKey = await Scatter.GetPublicKey(Scatter.Blockchains.EOSIO);

            Console.WriteLine(await Scatter.LinkAccount(pubKey, network));
        }

        //TODO blocking
        [TestMethod]
        public async Task HasAccountFor()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.HasAccountFor(network));
        }

        [TestMethod]
        public async Task SuggestNetwork()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.SuggestNetwork(network));
        }

        //TODO parse "error": "to account does not exist"
        [TestMethod]
        public async Task RequestTransfer()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.RequestTransfer(network, "tester112345", "tester212345", "0.0001 EOS"));
        }

        //TODO blocking
        [TestMethod]
        public async Task RequestSignature()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);

            //var identity = await GetIdentityFromScatter();

            Console.WriteLine(await Scatter.RequestSignature(new {
                network,
                blockchain = Scatter.Blockchains.EOSIO,
                requiredFields = new List<object>(),
                messages = new List<object>(),
                //payload.messages
                //.map(message => message.authorization
                //    .map(auth => `${ auth.actor}@${ auth.permission}`))
                origin = "SCATTER-SHARP"
            }));
        }

        //TODO check this
        [TestMethod]
        public async Task CreateTransaction()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.CreateTransaction(Scatter.Blockchains.EOSIO, new List<object>(), "tester112345", network));

              //  {
              //      "network": {
              //                  "name": null,
              //  "blockchain": "eos",
              //  "host": "nodes.eos42.io",
              //  "port": 443,
              //  "chainId": "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906"
              //              },
              //"blockchain": "eos",
              //"requiredFields": []
              //  }
        }

        private async Task<Api.Identity> GetIdentityFromScatter()
        {
            return await Scatter.GetIdentity(new Api.IdentityRequiredFields()
            {
                Accounts = new List<Api.Network>()
                {
                    network
                },
                Location = new List<Api.LocationFields>(),
                Personal = new List<Api.PersonalFields>()
            });
        }
    }
}
