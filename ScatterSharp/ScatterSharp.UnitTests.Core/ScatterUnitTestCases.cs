using Newtonsoft.Json;
using ScatterSharp.Core.Api;
using ScatterSharp.Core.Helpers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScatterSharp.UnitTests.Core
{
    public class ScatterUnitTestCases
    {
        private Scatter Scatter { get; set; }
        private Network Network { get; set; }

        public ScatterUnitTestCases(Scatter scatter, Network network)
        {
            Scatter = scatter;
            Network = network;
        }

        public async Task Connect()
        {
            await Scatter.Connect();
        }

        public async Task<string> GetVersion()
        {
            await Scatter.Connect();
            return await Scatter.GetVersion();
        }

        public async Task<Identity> GetIdentity()
        {
            await Scatter.Connect();
            return await GetIdentityFromScatter();
        }

        public async Task<Identity> GetIdentityFromPermissions()
        {
            await Scatter.Connect();
            return await Scatter.GetIdentityFromPermissions();
        }

        public async Task<bool> ForgetIdentity()
        {
            await Scatter.Connect();
            return await Scatter.ForgetIdentity();
        }

        public async Task<string> Authenticate()
        {
            await Scatter.Connect();
            var identity = await GetIdentityFromScatter();
            return await Scatter.Authenticate(UtilsHelper.RandomNumber());
        }

        public async Task<string> GetArbitrarySignature()
        {
            await Scatter.Connect();
            var identity = await GetIdentityFromScatter();
            return await Scatter.GetArbitrarySignature(identity.publicKey, "HELLO WORLD!");
        }

        public async Task<string> GetPublicKey()
        {
            await Scatter.Connect();
            return await Scatter.GetPublicKey(Scatter.Blockchains.EOSIO);
        }

        public async Task<bool> LinkAccount()
        {
            await Scatter.Connect();
            var identity = await GetIdentityFromScatter();
            var account = identity.accounts.First();
            return await Scatter.LinkAccount(new LinkAccount() {
                publicKey = account.publicKey,
                name = account.name,
                authority = account.authority
            });
        }

        public async Task<bool> HasAccountFor()
        {
            await Scatter.Connect();
            return await Scatter.HasAccountFor();
        }

        public async Task<bool> SuggestNetwork()
        {
            await Scatter.Connect();
            return await Scatter.SuggestNetwork();
        }

        //TODO parse "error": "to account does not exist"

        public async Task<object> RequestTransfer()
        {
            await Scatter.Connect();
            return await Scatter.RequestTransfer("tester112345", "tester212345", "0.0001 EOS");
        }

        public async Task<SignaturesResult> RequestSignature()
        {
            await Scatter.Connect();
            var identity = await GetIdentityFromScatter();
            return await Scatter.RequestSignature(new
            {
                Network,
                blockchain = Scatter.Blockchains.EOSIO,
                requiredFields = new List<object>(),
                //TODO add transaction
                origin = Scatter.AppName
            });
        }

        public async Task AddToken()
        {
            await Scatter.Connect();
            var identity = await GetIdentityFromScatter();
            await Scatter.AddToken(new Token()
            {
                name = "EOS",
                symbol = "EOS",
                contract = "eosio.token"
            });
        }

        public async Task<string> GetEncryptionKey()
        {
            await Scatter.Connect();

            var fromKey = await Scatter.GetPublicKey(Scatter.Blockchains.EOSIO);
            var toKey = await Scatter.GetPublicKey(Scatter.Blockchains.EOSIO);
            var r = new Random();

            return await Scatter.GetEncryptionKey(fromKey, toKey, (UInt64)r.Next());
        }

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

        public async Task<bool> SimulateSendSecretMessage()
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

            var roundtrip = CryptoHelper.AesDecrypt(encryptionKeyBBytes, encrypted);
            Console.WriteLine("Round Trip: {0}", roundtrip);

            return encryptionKeyA == encryptionKeyB;
        }

        private async Task<Identity> GetIdentityFromScatter()
        {
            return await Scatter.GetIdentity(new IdentityRequiredFields()
            {
                accounts = new List<Network>()
                {
                    Network
                },
                location = new List<LocationFields>(),
                personal = new List<PersonalFields>()
            });
        }
    }
}
