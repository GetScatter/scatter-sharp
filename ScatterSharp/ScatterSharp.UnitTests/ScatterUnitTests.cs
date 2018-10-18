using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using ScatterSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScatterSharp.UnitTests
{
    [TestClass]
    public class ScatterUnitTests
    {
        //mainnet
        //public static readonly Api.Network network = new Api.Network()
        //{
        //    Blockchain = Scatter.Blockchains.EOSIO,
        //    Host = "nodes.eos42.io",
        //    Port = 443,
        //    ChainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906"
        //};

        //Jungle testnet
        public static readonly Api.Network network = new Api.Network()
        {
            Blockchain = Scatter.Blockchains.EOSIO,
            Host = "jungle.cryptolions.io",
            Port = 18888,
            ChainId = "cf057bbfb72640471fd910bcb67639c22df9f92470936cddc1ade0e2f2e7dc4f"
        };

        public Scatter Scatter { get; set; }

        public ScatterUnitTests()
        {
            Scatter = new Scatter("SCATTER-SHARP", network);
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task Connect()
        {
            await Scatter.Connect();
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task GetVersion()
        {
            await Scatter.Connect();
            Console.WriteLine(await Scatter.GetVersion());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task GetIdentity()
        {
            await Scatter.Connect();
            Console.WriteLine(JsonConvert.SerializeObject(await GetIdentityFromScatter()));
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task GetIdentityFromPermissions()
        {
            await Scatter.Connect();
            Console.WriteLine(await Scatter.GetIdentityFromPermissions());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task ForgetIdentity()
        {
            await Scatter.Connect();
            Console.WriteLine(await Scatter.ForgetIdentity());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task Authenticate()
        {
            await Scatter.Connect();

            var identity = await GetIdentityFromScatter();

            Console.WriteLine(await Scatter.Authenticate(UtilsHelper.RandomNumber()));
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task GetArbitrarySignature()
        {
            await Scatter.Connect();

            var identity = await GetIdentityFromScatter();

            Console.WriteLine(await Scatter.GetArbitrarySignature(identity.PublicKey, "HELLO WORLD!"));
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task GetPublicKey()
        {
            await Scatter.Connect();
            Console.WriteLine(await Scatter.GetPublicKey(Scatter.Blockchains.EOSIO));
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task LinkAccount()
        {
            await Scatter.Connect();

            var pubKey = await Scatter.GetPublicKey(Scatter.Blockchains.EOSIO);

            Console.WriteLine(await Scatter.LinkAccount(pubKey));
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task HasAccountFor()
        {
            await Scatter.Connect();
            Console.WriteLine(await Scatter.HasAccountFor());
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task SuggestNetwork()
        {
            await Scatter.Connect();
            Console.WriteLine(await Scatter.SuggestNetwork());
        }

        //TODO parse "error": "to account does not exist"
        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task RequestTransfer()
        {
            await Scatter.Connect();
            Console.WriteLine(await Scatter.RequestTransfer("tester112345", "tester212345", "0.0001 EOS"));
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task RequestSignature()
        {
            await Scatter.Connect();

            var identity = await GetIdentityFromScatter();

            Console.WriteLine(await Scatter.RequestSignature(new {
                network,
                blockchain = Scatter.Blockchains.EOSIO,
                requiredFields = new List<object>(),
                //TODO add transaction
                origin = Scatter.AppName
            }));
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task GetEncryptionKey()
        {
            await Scatter.Connect();

            var fromKey = await Scatter.GetPublicKey(Scatter.Blockchains.EOSIO);
            var toKey = await Scatter.GetPublicKey(Scatter.Blockchains.EOSIO);
            var r = new Random();

            Console.WriteLine(await Scatter.GetEncryptionKey(fromKey, toKey, (UInt64)r.Next()));
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task OneWayEncryptDecrypt()
        {
            await Scatter.Connect();

            var fromKey = await Scatter.GetPublicKey(Scatter.Blockchains.EOSIO);
            var toKey = await Scatter.GetPublicKey(Scatter.Blockchains.EOSIO);
            var r = new Random();
            var encryptionKey = await Scatter.GetEncryptionKey(fromKey, toKey, (UInt64)r.Next());
            var encryptionKeyBytes = UtilsHelper.HexStringToByteArray(encryptionKey);

            string text = "Hello crypto secret message!";
            var encrypted = CryptoHelper.AesEncrypt(encryptionKeyBytes, text);
            var roundtrip = CryptoHelper.AesDecrypt(encryptionKeyBytes, encrypted);

            Console.WriteLine("FromKey:    {0}", fromKey);
            Console.WriteLine("ToKey:      {0}", toKey);
            Console.WriteLine("Original:   {0}", text);
            Console.WriteLine("Encrypted:  {0}", Encoding.UTF8.GetString(encrypted));
            Console.WriteLine("Round Trip: {0}", roundtrip);
        }

        [TestMethod]
        [TestCategory("Scatter Tests")]
        public async Task SimulateSendSecretMessage()
        {
            await Scatter.Connect();

            var fromKey = await Scatter.GetPublicKey(Scatter.Blockchains.EOSIO);
            var toKey = await Scatter.GetPublicKey(Scatter.Blockchains.EOSIO);
            var r = new Random();
            var nonce = (UInt64)r.Next();
            var text = "Hello crypto secret message!";
            var encryptionKeyA = await Scatter.GetEncryptionKey(fromKey, toKey, nonce);
            var encryptionKeyABytes = UtilsHelper.HexStringToByteArray(encryptionKeyA);

            Console.WriteLine("FromKey:    {0}", fromKey);
            Console.WriteLine("ToKey:      {0}", toKey);
            Console.WriteLine("Original:   {0}", text);

            var encrypted = CryptoHelper.AesEncrypt(encryptionKeyABytes, text);
            Console.WriteLine("Encrypted:  {0}", Encoding.UTF8.GetString(encrypted));

            //...Send over the wire...

            var encryptionKeyB = await Scatter.GetEncryptionKey(toKey, fromKey, nonce);
            var encryptionKeyBBytes = UtilsHelper.HexStringToByteArray(encryptionKeyB);

            Console.WriteLine("A_PVT_KEY + B_PUB_KEY:    {0}", encryptionKeyA);
            Console.WriteLine("B_PVT_KEY + A_PUB_KEY:    {0}", encryptionKeyB);

            Assert.IsTrue(encryptionKeyA == encryptionKeyB);

            var roundtrip = CryptoHelper.AesDecrypt(encryptionKeyBBytes, encrypted);
            Console.WriteLine("Round Trip: {0}", roundtrip);
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
