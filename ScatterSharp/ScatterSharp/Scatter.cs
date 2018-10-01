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
            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "getVersion",
                Payload = new { origin = AppName }
            });

            return result as string;
        }

        public async Task<string> GetIdentity(IdentityRequiredFields requiredFields)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "getOrRequestIdentity",
                Payload = new { fields = requiredFields, origin = AppName }
            });

            Identity = result as string;

            return Identity;
        }

        public async Task<string> GetIdentityFromPermissions()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "identityFromPermissions",
                Payload = new { origin = AppName }
            });

            Identity = result as string;

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

            return result;
        }

        public async Task<object> GetArbitrarySignature(string publicKey, string data, string whatfor = "", bool isHash = false)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "requestArbitrarySignature",
                Payload = new { publicKey, data, whatfor, isHash, origin = AppName }
            });

            return result;
        }

        public async Task<object> GetPublicKey(string blockchain)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "getPublicKey",
                Payload = new { blockchain } //TODO keypair
            });

            return result;
        }

        public async Task<object> LinkAccount(string publicKey, Network network)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "linkAccount",
                Payload = new { publicKey, network, origin = AppName }
            });

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

            return result;
        }

        public async Task<object> RequestTransfer(Network network, string to, string amount, object options = null)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "requestTransfer",
                Payload = new { network, to, amount, options }
            });

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

            return result;
        }

        public async Task<object> CreateTransaction(string blockchain, List<object> actions, string account, Network network)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                Type = "createTransaction",
                Payload = new { blockchain, actions, account, network }
            });

            return result;
        }
    }
}
