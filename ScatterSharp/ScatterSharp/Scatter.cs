using ScatterSharp.Api;
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

        public Scatter(string appName)
        {
            SocketService = new SocketService(new MemoryStorageProvider(), appName);
            AppName = appName;
        }

        public void ThrowNoAuth()
        {
            if(!SocketService.IsConnected())
                throw new Exception("Connect and Authenticate first - scatter.connect( appName )");
        }

        public async Task Connect(string host, CancellationToken? cancellationToken = null)
        {
            await SocketService.Link(new Uri(string.Format(WSURI, host)), cancellationToken);
            this.Identity = await this.GetIdentityFromPermissions();
        }

        public async Task<string> GetVersion()
        {
            var result = await SocketService.SendApiRequest(new ApiRequest()
            {
                Type = "getVersion",
                Payload = new { origin = AppName }
            });

            return result as string;
        }

        public async Task<string> GetIdentity(ApiRequiredFields requiredFields)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new ApiRequest()
            {
                Type = "getOrRequestIdentity",
                Payload = new { fields = requiredFields }
            });

            Identity = result as string;

            return Identity;
        }

        public async Task<string> GetIdentityFromPermissions()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new ApiRequest()
            {
                Type = "identityFromPermissions",
                Payload = new {}
            });

            Identity = result as string;

            return Identity;
        }

        public async Task<object> ForgetIdentity()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new ApiRequest()
            {
                Type = "forgetIdentity",
                Payload = new {}
            });

            Identity = null;
            return result;
        }

        public async Task<object> Authenticate(string nonce)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new ApiRequest()
            {
                Type = "authenticate",
                Payload = new { nonce }
            });

            return result;
        }

        public async Task<object> GetArbitrarySignature(string publicKey, string data, string whatfor = "", bool isHash = false)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new ApiRequest()
            {
                Type = "requestArbitrarySignature",
                Payload = new { publicKey, data, whatfor, isHash }
            });

            return result;
        }

        public async Task<object> GetPublicKey(string blockchain)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new ApiRequest()
            {
                Type = "getPublicKey",
                Payload = new { blockchain }
            });

            return result;
        }

        public async Task<object> LinkAccount(string publicKey, string network)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new ApiRequest()
            {
                Type = "linkAccount",
                Payload = new { publicKey, network }
            });

            return result;
        }

        public async Task<object> HasAccountFor(string network)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new ApiRequest()
            {
                Type = "hasAccountFor",
                Payload = new { network }
            });

            return result;
        }

        public async Task<object> SuggestNetwork(string network)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new ApiRequest()
            {
                Type = "requestAddNetwork",
                Payload = new { network }
            });

            return result;
        }

        public async Task<object> RequestTransfer(string network, string to, string amount, object options = null)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new ApiRequest()
            {
                Type = "requestTransfer",
                Payload = new { network, to, amount, options }
            });

            return result;
        }

        public async Task<object> RequestSignature(object payload)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new ApiRequest()
            {
                Type = "requestSignature",
                Payload = payload
            });

            return result;
        }

        public async Task<object> CreateTransaction(string blockchain, List<object> actions, string account, string network)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new ApiRequest()
            {
                Type = "createTransaction",
                Payload = new { blockchain, actions, account, network }
            });

            return result;
        }
    }
}
