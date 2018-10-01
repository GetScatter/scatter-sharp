using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public string SCATTER_DESKTOP_WS_HOST = "127.0.0.1:50005";
        public string BLOCKCHAIN = "eos";

        //mainnet
        public string ENDPOINT_HOST = "nodes.eos42.io";
        public string ENDPOINT_PORT = "443";
        public string ENDPOINT_CHAINID = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906";
        public string TEST_PUBKEY = "TEST_PUBKEY";

        //btuga testnet
        //public string ENDPOINT_HOST = "nodeos01.btuga.io";
        //public string ENDPOINT_PORT = "443";
        //public string ENDPOINT_CHAINID = "cf057bbfb72640471fd910bcb67639c22df9f92470936cddc1ade0e2f2e7dc4f";
        //public string TEST_PRIVKEY = "5K57oSZLpfzePvQNpsLS6NfKXLhhRARNU13q6u2ZPQCGHgKLbTA";
        //public string TEST_PUBKEY = "EOS8Q8CJqwnSsV4A6HDBEqmQCqpQcBnhGME1RUvydDRnswNngpqfr";

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
            Console.WriteLine(await Scatter.GetIdentity(new Api.ApiRequiredFields() {
                Accounts = new List<Api.ApiField>()
                {
                    new Api.ApiField()
                    {
                        Blockchain = BLOCKCHAIN,
                        Host = ENDPOINT_HOST,
                        Port = ENDPOINT_PORT,
                        ChainId = ENDPOINT_CHAINID
                    }
                }
            }));
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

        [TestMethod]
        public async Task GetArbitrarySignature()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.GetArbitrarySignature(TEST_PUBKEY, "HELLO WORLD!"));
        }

        [TestMethod]
        public async Task GetPublicKey()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.GetPublicKey(BLOCKCHAIN));
        }

        [TestMethod]
        public async Task LinkAccount()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.LinkAccount(TEST_PUBKEY, BLOCKCHAIN));
        }

        [TestMethod]
        public async Task HasAccountFor()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.HasAccountFor(BLOCKCHAIN));
        }

        [TestMethod]
        public async Task SuggestNetwork()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.SuggestNetwork(BLOCKCHAIN));
        }

        [TestMethod]
        public async Task RequestTransfer()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            Console.WriteLine(await Scatter.RequestTransfer(BLOCKCHAIN, "tester112345", "tester212345", "1.0000 EOS"));
        }

        [TestMethod]
        public async Task RequestSignature()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task CreateTransaction()
        {
            await Scatter.Connect(SCATTER_DESKTOP_WS_HOST);
            throw new NotImplementedException();
        }
    }
}
