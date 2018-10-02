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
        public static readonly string BLOCKCHAIN = "eos";

        //mainnet
        public static readonly string ENDPOINT_HOST = "nodes.eos42.io";
        public static readonly int ENDPOINT_PORT = 443;
        public static readonly string ENDPOINT_CHAINID = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906";
        public static readonly string TEST_PUBKEY = "TEST_PUBKEY";

        //btuga testnet
        //public static readonly string ENDPOINT_HOST = "nodeos01.btuga.io";
        //public static readonly int    ENDPOINT_PORT = 443;
        //public static readonly string ENDPOINT_CHAINID = "cf057bbfb72640471fd910bcb67639c22df9f92470936cddc1ade0e2f2e7dc4f";
        //public static readonly string TEST_PRIVKEY = "5K57oSZLpfzePvQNpsLS6NfKXLhhRARNU13q6u2ZPQCGHgKLbTA";
        //public static readonly string TEST_PUBKEY = "EOS8Q8CJqwnSsV4A6HDBEqmQCqpQcBnhGME1RUvydDRnswNngpqfr";

        public static readonly Api.Network network = new Api.Network()
        {
            Blockchain = BLOCKCHAIN,
            Host = ENDPOINT_HOST,
            Port = ENDPOINT_PORT,
            ChainId = ENDPOINT_CHAINID
        };

        public Scatter Scatter { get; set; }

        public ScatterUnitTests()
        {
            Scatter = new Scatter("TESTAPP");
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
            //mainnet
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(JsonConvert.SerializeObject(await Scatter.GetIdentity(new Api.IdentityRequiredFields()
            {
                Accounts = new List<Api.Network>()
                {
                    network
                },
                Location = new List<Api.LocationFields>(),
                Personal = new List<Api.PersonalFields>()
            })));
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
            Console.WriteLine(await Scatter.Authenticate(UtilsHelper.RandomNumber()));
        }

        //TODO no identity
        [TestMethod]
        public async Task GetArbitrarySignature()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.GetArbitrarySignature(TEST_PUBKEY, "HELLO WORLD!"));
        }

        //TODO blocking
        [TestMethod]
        public async Task GetPublicKey()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.GetPublicKey(BLOCKCHAIN));
        }

        //TODO invalid public key
        [TestMethod]
        public async Task LinkAccount()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.LinkAccount(TEST_PUBKEY, network));
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

        //TODO blocking
        [TestMethod]
        public async Task RequestTransfer()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.RequestTransfer(network, "tester112345", "tester212345", "1.0000 EOS"));
        }

        //TODO blocking
        [TestMethod]
        public async Task RequestSignature()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.RequestSignature(new { }));
        }

        //TODO blocking
        [TestMethod]
        public async Task CreateTransaction()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.CreateTransaction(BLOCKCHAIN, new List<object>(), "tester112345", network));
        }
    }
}
