using Cryptography.ECDSA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using ScatterSharp.Core;
using ScatterSharp.Core.Api;
using ScatterSharp.Core.Helpers;
using ScatterSharp.Core.Storage;
using ScatterSharp.UnitTests.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScatterSharp.UnitTests
{
    [TestClass]
    public class ScatterUnitTests
    {
        //mainnet
        public static readonly Network network = new Network()
        {
            blockchain = ScatterConstants.Blockchains.EOSIO,
            host = "nodes.eos42.io",
            port = 443,
            chainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906"
        };

        //Jungle testnet
        //public static readonly Network network = new Network()
        //{
        //    blockchain = Scatter.Blockchains.EOSIO,
        //    host = "jungle.cryptolions.io",
        //    port = 18888,
        //    chainId = "cf057bbfb72640471fd910bcb67639c22df9f92470936cddc1ade0e2f2e7dc4f"
        //};

        public ScatterUnitTestCases ScatterUnitTestCases { get; set; }

        public ScatterUnitTests()
        {
            var storageProvider = new MemoryStorageProvider();
            storageProvider.SetAppkey(UtilsHelper.ByteArrayToHexString(Sha256Manager.GetHash(Encoding.UTF8.GetBytes("appkey:0a182c0d054b6fd9f9361c82fcd040b46c41a6f61952a3ea"))));

            var scatter = new Scatter(new ScatterConfigurator()
            {
                AppName = "SCATTER-SHARP",
                Network = network,
                StorageProvider = storageProvider
            });

            ScatterUnitTestCases = new ScatterUnitTestCases(scatter, network);
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task Connect()
        {
            await ScatterUnitTestCases.Connect();
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task GetVersion()
        {
            Console.WriteLine(await ScatterUnitTestCases.GetVersion());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task GetIdentity()
        {
            Console.WriteLine(JsonConvert.SerializeObject(await ScatterUnitTestCases.GetIdentity()));
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task GetIdentityFromPermissions()
        {
            Console.WriteLine(await ScatterUnitTestCases.GetIdentityFromPermissions());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task ForgetIdentity()
        {
            Console.WriteLine(await ScatterUnitTestCases.ForgetIdentity());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task Authenticate()
        {
            Console.WriteLine(await ScatterUnitTestCases.Authenticate());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task GetArbitrarySignature()
        {
            Console.WriteLine(await ScatterUnitTestCases.GetArbitrarySignature());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task GetPublicKey()
        {
            Console.WriteLine(await ScatterUnitTestCases.GetPublicKey());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task LinkAccount()
        {
            Console.WriteLine(await ScatterUnitTestCases.LinkAccount());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task HasAccountFor()
        {
            Console.WriteLine(await ScatterUnitTestCases.HasAccountFor());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task SuggestNetwork()
        {
            Console.WriteLine(await ScatterUnitTestCases.SuggestNetwork());
        }

        //TODO parse "error": "to account does not exist"
        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task RequestTransfer()
        {
            Console.WriteLine(await ScatterUnitTestCases.RequestTransfer());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task RequestSignature()
        {
            Console.WriteLine(await ScatterUnitTestCases.RequestSignature());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task AddToken()
        {
            await ScatterUnitTestCases.AddToken();
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task GetEncryptionKey()
        {
            Console.WriteLine(await ScatterUnitTestCases.GetEncryptionKey());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task OneWayEncryptDecrypt()
        {
            await ScatterUnitTestCases.OneWayEncryptDecrypt();
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task SimulateSendSecretMessage()
        {
            Assert.IsTrue(await ScatterUnitTestCases.SimulateSendSecretMessage());
        }
    }
}
