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

            SocketService.On(ScatterConstants.Events.Disconnected, (payload) =>
            {
                Identity = null;
            });

            SocketService.On(ScatterConstants.Events.LoggedOut, async (payload) =>
            {
                await GetIdentityFromPermissions();
            });
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
            var result = await SocketService.SendApiRequest<ApiBase, string>(new Request<ApiBase>()
            {
                type = "getVersion",
                payload = new ApiBase()
                {
                    origin = AppName
                }
            });

            return result;
        }

        public async Task<Identity> GetIdentity(IdentityRequiredFields requiredFields)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<IdentityRequest, Identity>(new Request<IdentityRequest>()
            {
                type = "getOrRequestIdentity",
                payload = new IdentityRequest()
                {
                    fields = requiredFields,
                    origin = AppName
                }
            });

            return Identity = result;
        }

        public async Task<Identity> GetIdentityFromPermissions()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<ApiBase, Identity>(new Request<ApiBase>()
            {
                type = "identityFromPermissions",
                payload = new ApiBase()
                {
                    origin = AppName
                }
            });

            if(result != null)
                Identity = result;

            return Identity;
        }

        public async Task<bool> ForgetIdentity()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<ApiBase, string>(new Request<ApiBase>()
            {
                type = "forgetIdentity",
                payload = new ApiBase()
                {
                    origin = AppName
                }
            });

            Identity = null;

            return bool.Parse(result);
        }

        public async Task<string> Authenticate(string nonce, string data = null, string publicKey = null)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<AuthenticateRequest, string>(new Request<AuthenticateRequest>()
            {
                type = "authenticate",
                payload = new AuthenticateRequest()
                {
                    nonce = nonce,
                    data = data,
                    publicKey = publicKey,
                    origin = AppName
                }
            });

            return result;
        }

        public async Task<string> GetArbitrarySignature(string publicKey, string data, string whatfor = "", bool isHash = false)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<ArbitrarySignatureRequest, string>(new Request<ArbitrarySignatureRequest>()
            {
                type = "requestArbitrarySignature",
                payload = new ArbitrarySignatureRequest()
                {
                    publicKey = publicKey,
                    data = data,
                    whatfor = whatfor,
                    isHash = isHash,
                    origin = AppName
                }
            });

            return result;
        }

        public async Task<string> GetPublicKey(string blockchain)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<GetPublicKeyRequest, string>(new Request<GetPublicKeyRequest>()
            {
                type = "getPublicKey",
                payload = new GetPublicKeyRequest()
                {
                    blockchain = blockchain,
                    origin = AppName
                }
            });

            return result;
        }

        public async Task<bool> LinkAccount(LinkAccount account)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<LinkAccountRequest, string>(new Request<LinkAccountRequest>()
            {
                type = "linkAccount",
                payload = new LinkAccountRequest()
                {
                    account = account,
                    network = Network,
                    origin = AppName
                }
            });

            return bool.Parse(result);
        }

        public async Task<bool> HasAccountFor()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<NetworkRequest, string>(new Request<NetworkRequest>()
            {
                type = "hasAccountFor",
                payload = new NetworkRequest()
                {
                    network = Network,
                    origin = AppName
                }
            });

            return bool.Parse(result);
        }

        public async Task<bool> SuggestNetwork()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<NetworkRequest, string>(new Request<NetworkRequest>()
            {
                type = "requestAddNetwork",
                payload = new NetworkRequest()
                {
                    network = Network,
                    origin = AppName
                }
            });

            return bool.Parse(result);
        }

        //TODO check
        public async Task<object> RequestTransfer(string to, string amount, object options = null)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<TransferRequest, object>(new Request<TransferRequest>()
            {
                type = "requestTransfer",
                payload = new TransferRequest()
                {
                    network = Network,
                    to = to,
                    amount = amount,
                    options = options,
                    origin = AppName
                }
            });

            return result;
        }

        public async Task<SignaturesResult> RequestSignature(object payload)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<object, SignaturesResult>(new Request<object>()
            {
                type = "requestSignature",
                payload = payload
            });

            return result;
        }

        public async Task<bool> AddToken(Token token)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<TokenRequest, string>(new Request<TokenRequest>()
            {
                type = "addToken",
                payload = new TokenRequest()
                {
                    token = token,
                    network = Network,
                    origin = AppName
                }
            });

            return bool.Parse(result);
        }

        public async Task<string> UpdateIdentity(string name, string kyc = null)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<UpdateIdentityRequest, string>(new Request<UpdateIdentityRequest>()
            {
                type = "updateIdentity",
                payload = new UpdateIdentityRequest()
                {
                    name = name,
                    kyc = kyc,
                    origin = AppName
                }
            });

            return result;
        }

        //TODO test on new branch
        public async Task<string> GetEncryptionKey(string fromPublicKey, string toPublicKey, UInt64 nonce)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<GetEncryptionKeyRequest, string>(new Request<GetEncryptionKeyRequest>()
            {
                type = "getEncryptionKey",
                payload = new GetEncryptionKeyRequest()
                {
                    fromPublicKey = fromPublicKey,
                    toPublicKey = toPublicKey,
                    nonce = nonce,
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
