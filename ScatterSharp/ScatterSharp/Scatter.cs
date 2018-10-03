using EosSharp;
using Newtonsoft.Json.Linq;
using ScatterSharp.Api;
using ScatterSharp.Providers;
using ScatterSharp.Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScatterSharp
{
    public class Scatter
    {
        private readonly string WSURI = "ws://{0}/socket.io/?EIO=3&transport=websocket";
        private SocketService SocketService { get; set; }
        private string AppName { get; set; }
        private string Identity { get; set; }

        public class Blockchains
        {
            public static readonly string EOSIO = "eos";
            public static readonly string ETH = "eth";
        };

        public Scatter(string appName)
        {
            SocketService = new SocketService(new MemoryStorageProvider(), appName);
            AppName = appName;
        }

        public async Task Connect(string host = "127.0.0.1:50005", CancellationToken? cancellationToken = null)
        {
            await SocketService.Link(new Uri(string.Format(WSURI, host)), cancellationToken);
            this.Identity = await this.GetIdentityFromPermissions();
        }

        public Eos Eos(Network network)
        {
            if (!SocketService.IsConnected())
                throw new Exception("Scatter is not connected.");

            if (network == null)
                throw new ArgumentNullException("network");

            string httpEndpoint = "";

            if (network.Port == 443)
                httpEndpoint += "https://" + network.Host + "/";
            else
                httpEndpoint += "http://" + network.Host + ":" + network.Port + "/";

            return new Eos(new EosConfigurator()
            {
                ChainId = network.ChainId,
                HttpEndpoint = httpEndpoint,
                SignProvider = new ScatterSignatureProvider(this)
            });
        }

        public async Task<string> GetVersion()
        {
            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "getVersion",
                Payload = new { origin = AppName }
            });

            ThrowOnApiError(result);

            return result.ToObject<string>();
        }

        public async Task<Identity> GetIdentity(IdentityRequiredFields requiredFields)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "getOrRequestIdentity",
                Payload = new { fields = requiredFields, origin = AppName }
            });

            ThrowOnApiError(result);

            //Identity = result.ToObject<Identity>();

            return result.ToObject<Identity>();
        }

        public async Task<string> GetIdentityFromPermissions()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "identityFromPermissions",
                Payload = new { origin = AppName }
            });

            ThrowOnApiError(result);

            Identity = result.ToObject<string>();

            return Identity;
        }

        public async Task<object> ForgetIdentity()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "forgetIdentity",
                Payload = new { origin = AppName }
            });

            ThrowOnApiError(result);

            Identity = null;
            return result;
        }

        public async Task<object> Authenticate(string nonce)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "authenticate",
                Payload = new { nonce, origin = AppName }
            });

            ThrowOnApiError(result);

            return result;
        }

        public async Task<string> GetArbitrarySignature(string publicKey, string data, string whatfor = "", bool isHash = false)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "requestArbitrarySignature",
                Payload = new { publicKey, data, whatfor, isHash, origin = AppName }
            });

            ThrowOnApiError(result);

            return result.ToObject<string>();
        }

        public async Task<string> GetPublicKey(string blockchain)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "getPublicKey",
                Payload = new { blockchain, origin = AppName }
            });

            ThrowOnApiError(result);

            return result.ToObject<string>();
        }

        public async Task<object> LinkAccount(string publicKey, Network network)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "linkAccount",
                Payload = new { publicKey, network, origin = AppName }
            });

            ThrowOnApiError(result);

            return result;
        }

        public async Task<object> HasAccountFor(Network network)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "hasAccountFor",
                Payload = new { network }
            });

            ThrowOnApiError(result);

            return result;
        }

        public async Task<object> SuggestNetwork(Network network)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "requestAddNetwork",
                Payload = new { network, origin = AppName }
            });

            ThrowOnApiError(result);

            return result;
        }

        public async Task<object> RequestTransfer(Network network, string to, string amount, object options = null)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "requestTransfer",
                Payload = new { network, to, amount, options, origin = AppName }
            });

            ThrowOnApiError(result);

            return result;
        }

        public async Task<object> RequestSignature(object payload)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "requestSignature",
                Payload = payload
            });

            ThrowOnApiError(result);

            return result;
        }

        public async Task<object> CreateTransaction(string blockchain, List<object> actions, string account, Network network)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "createTransaction",
                Payload = new { blockchain, actions, account, network, origin = AppName }
            });

            ThrowOnApiError(result);

            return result;
        }

        #region Utils
        private void ThrowNoAuth()
        {
            if (!SocketService.IsConnected())
                throw new Exception("Connect and Authenticate first - scatter.connect( appName )");
        }

        private static void ThrowOnApiError(JToken result)
        {
            if (result.Type != JTokenType.Object ||
               result.SelectToken("isError") == null)
                return;

            var apiError = result.ToObject<ApiError>();

            if (apiError != null)
                throw new Exception(apiError.Message);
        }
        #endregion
    }

}
