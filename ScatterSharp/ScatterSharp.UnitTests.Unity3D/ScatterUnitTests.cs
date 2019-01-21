using Cryptography.ECDSA;
using Newtonsoft.Json;
using ScatterSharp.Core.Api;
using ScatterSharp.Core.Helpers;
using ScatterSharp.Core.Storage;
using ScatterSharp.UnitTests.Core;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ScatterSharp.UnitTests
{
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
        public static readonly Network network = new Network()
        {
            blockchain = Scatter.Blockchains.EOSIO,
            host = "jungle.cryptolions.io",
            port = 18888,
            chainId = "cf057bbfb72640471fd910bcb67639c22df9f92470936cddc1ade0e2f2e7dc4f"
        };

        public ScatterUnitTestCases ScatterUnitTestCases { get; set; }

        public ScatterUnitTests()
        {
            var storageProvider = new MemoryStorageProvider();
            storageProvider.SetAppkey(UtilsHelper.ByteArrayToHexString(Sha256Manager.GetHash(Encoding.UTF8.GetBytes("appkey:0a182c0d054b6fd9f9361c82fcd040b46c41a6f61952a3ea"))));

            var scatter = new Scatter("SCATTER-SHARP", network, storageProvider);
            ScatterUnitTestCases = new ScatterUnitTestCases(scatter, network);
        }

        public async Task Connect()
        {
            bool success = false;

            try
            {
                await ScatterUnitTestCases.Connect();
                success = true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test Connect run successfuly.");
            else
                Console.WriteLine("Test Connect run failed.");
        }

        public async Task GetVersion()
        {
            bool success = false;

            try
            {
                
                Console.WriteLine(JsonConvert.SerializeObject(await ScatterUnitTestCases.GetVersion()));
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test GetVersion run successfuly.");
            else
                Console.WriteLine("Test GetVersion run failed.");
        }

        public async Task GetIdentity()
        {
            bool success = false;

            try
            {
                Console.WriteLine(JsonConvert.SerializeObject(await ScatterUnitTestCases.GetIdentity()));
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test GetIdentity run successfuly.");
            else
                Console.WriteLine("Test GetIdentity run failed.");
        }

        public async Task GetIdentityFromPermissions()
        {
            bool success = false;

            try
            {
                Console.WriteLine(await ScatterUnitTestCases.GetIdentityFromPermissions());
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test GetIdentityFromPermissions run successfuly.");
            else
                Console.WriteLine("Test GetIdentityFromPermissions run failed.");
        }

        public async Task ForgetIdentity()
        {
            bool success = false;

            try
            {
                Console.WriteLine(await ScatterUnitTestCases.ForgetIdentity());
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test ForgetIdentity run successfuly.");
            else
                Console.WriteLine("Test ForgetIdentity run failed.");
        }

        public async Task Authenticate()
        {
            bool success = false;

            try
            {
                Console.WriteLine(await ScatterUnitTestCases.Authenticate());
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test Authenticate run successfuly.");
            else
                Console.WriteLine("Test Authenticate run failed.");
        }

        public async Task GetArbitrarySignature()
        {
            bool success = false;

            try
            {
                Console.WriteLine(await ScatterUnitTestCases.GetArbitrarySignature());
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test GetArbitrarySignature run successfuly.");
            else
                Console.WriteLine("Test GetArbitrarySignature run failed.");
        }

        public async Task GetPublicKey()
        {
            bool success = false;

            try
            {
                Console.WriteLine(await ScatterUnitTestCases.GetPublicKey());
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test GetPublicKey run successfuly.");
            else
                Console.WriteLine("Test GetPublicKey run failed.");
        }

        public async Task LinkAccount()
        {
            bool success = false;

            try
            {
                Console.WriteLine(await ScatterUnitTestCases.LinkAccount());
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test LinkAccount run successfuly.");
            else
                Console.WriteLine("Test LinkAccount run failed.");
        }

        public async Task HasAccountFor()
        {
            bool success = false;

            try
            {
                Console.WriteLine(await ScatterUnitTestCases.HasAccountFor());
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test HasAccountFor run successfuly.");
            else
                Console.WriteLine("Test HasAccountFor run failed.");
        }

        public async Task SuggestNetwork()
        {
            bool success = false;

            try
            {
                Console.WriteLine(await ScatterUnitTestCases.SuggestNetwork());
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test SuggestNetwork run successfuly.");
            else
                Console.WriteLine("Test SuggestNetwork run failed."); 
        }

        //TODO parse "error": "to account does not exist"
        public async Task RequestTransfer()
        {
            bool success = false;

            try
            {
                Console.WriteLine(await ScatterUnitTestCases.RequestTransfer());
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test RequestTransfer run successfuly.");
            else
                Console.WriteLine("Test RequestTransfer run failed.");
        }

        public async Task RequestSignature()
        {
            bool success = false;

            try
            {
                Console.WriteLine(await ScatterUnitTestCases.RequestSignature());
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test RequestSignature run successfuly.");
            else
                Console.WriteLine("Test RequestSignature run failed.");
        }

        public async Task AddToken()
        {
            bool success = false;

            try
            {
                await ScatterUnitTestCases.AddToken();
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test AddToken run successfuly.");
            else
                Console.WriteLine("Test AddToken run failed.");
        }

        public async Task GetEncryptionKey()
        {
            bool success = false;

            try
            {
                Console.WriteLine(await ScatterUnitTestCases.GetEncryptionKey());
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test GetEncryptionKey run successfuly.");
            else
                Console.WriteLine("Test GetEncryptionKey run failed.");
        }

        public async Task OneWayEncryptDecrypt()
        {
            bool success = false;

            try
            {
                await ScatterUnitTestCases.OneWayEncryptDecrypt();
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test OneWayEncryptDecrypt run successfuly.");
            else
                Console.WriteLine("Test OneWayEncryptDecrypt run failed.");
        }

        public async Task SimulateSendSecretMessage()
        {
            bool success = false;

            try
            {
                success = await ScatterUnitTestCases.SimulateSendSecretMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }

            if (success)
                Console.WriteLine("Test OneWayEncryptDecrypt run successfuly.");
            else
                Console.WriteLine("Test OneWayEncryptDecrypt run failed.");
        }
    }
}
