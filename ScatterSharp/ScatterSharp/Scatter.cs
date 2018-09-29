using ScatterSharp.Api;
using ScatterSharp.Storage;
using System;
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

        public Task Connect(string host, CancellationToken? cancellationToken = null)
        {
            return SocketService.Link(new Uri(string.Format(WSURI, host)), cancellationToken);
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

        public async Task<object> GetArbitrarySignature(/*publicKey, data, whatfor = '', isHash = false*/)
        {
            throw new NotImplementedException();
            //throwNoAuth();
            //return SocketService.sendApiRequest({
            //    type: 'requestArbitrarySignature',
            //payload:
            //    {
            //        publicKey,
            //    data,
            //    whatfor,
            //    isHash
            //}
            //});
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

        public async Task<object> RequestTransfer(/*network, to, amount, options = { }*/)
        {
            throw new NotImplementedException();
            //const payload = { network, to, amount, options };
            //return SocketService.sendApiRequest({
            //    type:'requestTransfer',
            //    payload
            //});
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

        public async Task<object> CreateTransaction(/*blockchain, actions, account, network*/)
        {
            throw new NotImplementedException();
            //throwNoAuth();
            //return SocketService.sendApiRequest({
            //    type: 'createTransaction',
            //        payload:
            //    {
            //        blockchain,
            //            actions,
            //            account,
            //            network
            //        }
            //});
        }
    }
}
