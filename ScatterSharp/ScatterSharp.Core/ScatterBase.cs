using ScatterSharp.Core;
using ScatterSharp.Core.Api;
using ScatterSharp.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace ScatterSharp
{
    public class ScatterBase : IScatter
    {
        private ISocketService SocketService { get; set; }

        public string AppName { get; set; }
        public Network Network { get; set; }
        public Identity Identity { get; set; }

        public ScatterBase(ScatterConfigurator config, ISocketService socketService)
        {
            if (config == null)
                config = new ScatterConfigurator();

            SocketService = socketService;
            AppName = config.AppName;
            Network = config.Network;
        }

        public void Dispose()
        {
            SocketService.Dispose();
        }

        public async Task Connect()
        {
            //Try connect with wss connection
            Uri wssURI = new Uri(string.Format(ScatterConstants.WSURI,
                                               ScatterConstants.WSS_PROTOCOL,
                                               ScatterConstants.WSS_HOST,
                                               ScatterConstants.WSS_PORT));

            bool linked = await SocketService.Link(wssURI);

            if (!linked)
            {
                //try normal ws connection
                Uri wsURI = new Uri(string.Format(ScatterConstants.WSURI,
                                                   ScatterConstants.WS_PROTOCOL,
                                                   ScatterConstants.WS_HOST,
                                                   ScatterConstants.WS_PORT));

                linked = await SocketService.Link(wsURI);

                //TODO check for errors
                if (!linked)
                    throw new Exception("socket not available.");
            }

            this.Identity = await this.GetIdentityFromPermissions();
        }

        public Network GetNetwork()
        {
            return Network;
        }

        public string GetAppName()
        {
            return AppName;
        }

        public async Task<string> GetVersion()
        {
            var result = await SocketService.SendApiRequest<string>(new Request()
            {
                type = "getVersion",
                payload = new { origin = AppName }
            });

            return result;
        }

        public async Task<Identity> GetIdentity(IdentityRequiredFields requiredFields)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<Identity>(new Request()
            {
                type = "getOrRequestIdentity",
                payload = new { fields = requiredFields, origin = AppName }
            });

            return Identity = result;
        }

        public async Task<Identity> GetIdentityFromPermissions()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<Identity>(new Request()
            {
                type = "identityFromPermissions",
                payload = new { origin = AppName }
            });

            if(result != null)
                Identity = result;

            return Identity;
        }

        public async Task<bool> ForgetIdentity()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<bool>(new Request()
            {
                type = "forgetIdentity",
                payload = new { origin = AppName }
            });

            Identity = null;

            return result;
        }

        public async Task<string> Authenticate(string nonce, string data = null, string publicKey = null)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<string>(new Request()
            {
                type = "authenticate",
                payload = new { nonce, data, publicKey, origin = AppName }
            });

            return result;
        }

        public async Task<string> GetArbitrarySignature(string publicKey, string data, string whatfor = "", bool isHash = false)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<string>(new Request()
            {
                type = "requestArbitrarySignature",
                payload = new { publicKey, data, whatfor, isHash, origin = AppName }
            });

            return result;
        }

        public async Task<string> GetPublicKey(string blockchain)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<string>(new Request()
            {
                type = "getPublicKey",
                payload = new { blockchain, origin = AppName }
            });

            return result;
        }

        public async Task<bool> LinkAccount(LinkAccount account)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<bool>(new Request()
            {
                type = "linkAccount",
                payload = new { account, network = Network, origin = AppName }
            });

            return result;
        }

        public async Task<bool> HasAccountFor()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<bool>(new Request()
            {
                type = "hasAccountFor",
                payload = new { network = Network, origin = AppName }
            });

            return result;
        }

        public async Task<bool> SuggestNetwork()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<bool>(new Request()
            {
                type = "requestAddNetwork",
                payload = new { network = Network, origin = AppName }
            });

            return result;
        }

        //TODO check
        public async Task<object> RequestTransfer(string to, string amount, object options = null)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<object>(new Request()
            {
                type = "requestTransfer",
                payload = new { network = Network, to, amount, options, origin = AppName }
            });

            return result;
        }

        public async Task<SignaturesResult> RequestSignature(object payload)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<SignaturesResult>(new Request()
            {
                type = "requestSignature",
                payload = payload
            });

            return result;
        }

        public async Task<bool> AddToken(Token token)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<bool>(new Request()
            {
                type = "addToken",
                payload = new { token, network = Network, origin = AppName }
            });

            return result;
        }

        //TODO test on new branch
        public async Task<string> GetEncryptionKey(string fromPublicKey, string toPublicKey, UInt64 nonce)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<string>(new Request()
            {
                type = "getEncryptionKey",
                payload = new
                {
                    fromPublicKey,
                    toPublicKey,
                    nonce,
                    origin = AppName
                }
            });

            return result;
        }

        public void On(string type, Action<object> callback)
        {
            SocketService.On(type, callback);
        }

        public void Off(string type)
        {
            SocketService.Off(type);
        }

        public void Off(string type, int index)
        {
            SocketService.Off(type, index);
        }

        public void Off(Action<object> callback)
        {
            SocketService.Off(callback);
        }

        public void Off(string type, Action<object> callback)
        {
            SocketService.Off(type, callback);
        }

        #region Utils
        private void ThrowNoAuth()
        {
            if (!SocketService.IsConnected())
                throw new Exception("Connect and Authenticate first - scatter.connect( appName )");
        }

        #endregion
    }

}
