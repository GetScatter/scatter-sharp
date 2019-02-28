using ScatterSharp.Core;
using ScatterSharp.Core.Api;
using ScatterSharp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScatterSharp
{
    /// <summary>
    /// Base implementation for scatter client
    /// </summary>
    public class ScatterBase : IScatter
    {
        private ISocketService SocketService { get; set; }

        public string AppName { get; set; }
        public Network Network { get; set; }
        public Identity Identity { get; set; }

        /// <summary>
        /// Constructor for scatter client with init configuration and socket service
        /// </summary>        
        /// <param name="config">Configuration object</param>
        /// <param name="socketService">Socket service implementation</param>
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

        /// <summary>
        /// Dispose socket service
        /// </summary>
        public void Dispose()
        {
            SocketService.Dispose();
        }

        /// <summary>
        /// Connect to scatter
        /// </summary>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task Connect(int? timeout = null)
        {
            //Try connect with wss connection
            Uri wssURI = new Uri(string.Format(ScatterConstants.WSURI,
                                               ScatterConstants.WSS_PROTOCOL,
                                               ScatterConstants.WSS_HOST,
                                               ScatterConstants.WSS_PORT));

            bool linked = await SocketService.Link(wssURI, timeout);

            if (!linked)
            {
                //try normal ws connection
                Uri wsURI = new Uri(string.Format(ScatterConstants.WSURI,
                                                   ScatterConstants.WS_PROTOCOL,
                                                   ScatterConstants.WS_HOST,
                                                   ScatterConstants.WS_PORT));

                linked = await SocketService.Link(wsURI, timeout);

                //TODO check for errors
                if (!linked)
                    throw new Exception("socket not available.");
            }

            this.Identity = await this.GetIdentityFromPermissions(timeout);
        }

        /// <summary>
        /// Get configured network
        /// </summary>
        /// <returns></returns>
        public Network GetNetwork()
        {
            return Network;
        }

        /// <summary>
        /// Get configured app name
        /// </summary>
        /// <returns></returns>
        public string GetAppName()
        {
            return AppName;
        }

        /// <summary>
        /// Get Scatter version
        /// </summary>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<string> GetVersion(int? timeout = null)
        {
            var result = await SocketService.SendApiRequest<ApiBase, string>(new Request<ApiBase>()
            {
                type = "getVersion",
                payload = new ApiBase()
                {
                    origin = AppName
                }
            }, timeout);

            return result;
        }

        /// <summary>
        /// Prompts the users for an Identity if there is no permission, otherwise returns the permission without a prompt based on origin.
        /// </summary>
        /// <param name="requiredFields">Optional required fields</param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<Identity> GetIdentity(IdentityRequiredFields requiredFields = null, int? timeout = null)
        {
            ThrowNoAuth();

            if(requiredFields == null)
            {
                requiredFields = new IdentityRequiredFields()
                {
                    accounts = new List<Network>()
                    {
                        Network
                    },
                    location = new List<LocationFields>(),
                    personal = new List<PersonalFields>()
                };
            }

            var result = await SocketService.SendApiRequest<IdentityRequest, Identity>(new Request<IdentityRequest>()
            {
                type = "getOrRequestIdentity",
                payload = new IdentityRequest()
                {
                    fields = requiredFields,
                    origin = AppName
                }
            }, timeout);

            return Identity = result;
        }

        /// <summary>
        /// Checks if an Identity has permissions and return the identity based on origin.
        /// </summary>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<Identity> GetIdentityFromPermissions(int? timeout = null)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<ApiBase, Identity>(new Request<ApiBase>()
            {
                type = "identityFromPermissions",
                payload = new ApiBase()
                {
                    origin = AppName
                }
            }, timeout);

            if(result != null)
                Identity = result;

            return Identity;
        }

        /// <summary>
        /// Removes the identity permission for an origin from the user's Scatter, effectively logging them out.
        /// </summary>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<bool> ForgetIdentity(int? timeout = null)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<ApiBase, string>(new Request<ApiBase>()
            {
                type = "forgetIdentity",
                payload = new ApiBase()
                {
                    origin = AppName
                }
            }, timeout);

            Identity = null;

            return bool.Parse(result);
        }

        /// <summary>
        /// Sign origin (appName) with the Identity's private key. Or custom data with custom publicKey
        /// </summary>
        /// <param name="nonce">entropy nonce</param>
        /// <param name="data">custom data</param>
        /// <param name="publicKey">custom publickey</param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<string> Authenticate(string nonce, string data = null, string publicKey = null, int? timeout = null)
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
            }, timeout);

            return result;
        }

        /// <summary>
        /// Sign arbitrary data with the constraint of max 12 words
        /// </summary>
        /// <param name="publicKey">publickey to request the private key signature</param>
        /// <param name="data">data to sign</param>
        /// <param name="whatfor">Optional reason for signature</param>
        /// <param name="isHash">is data a sha256 hash</param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<string> GetArbitrarySignature(string publicKey, string data, string whatfor = "", bool isHash = false, int? timeout = null)
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
            }, timeout);

            return result;
        }

        /// <summary>
        /// Allows apps to request that the user provide a user-selected Public Key to the app. ( ONBOARDING HELPER )
        /// </summary>
        /// <param name="blockchain"></param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<string> GetPublicKey(string blockchain, int? timeout = null)
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
            }, timeout);

            return result;
        }

        /// <summary>
        /// Allows the app to suggest that the user link new accounts on top of public keys ( ONBOARDING HELPER )
        /// </summary>
        /// <param name="account"></param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<bool> LinkAccount(LinkAccount account, int? timeout = null)
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
            }, timeout);

            return bool.Parse(result);
        }

        /// <summary>
        /// Allows dapps to see if a user has an account for a specific blockchain. DOES NOT PROMPT and does not return an actual account, just a boolean.
        /// </summary>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<bool> HasAccountFor(int? timeout = null)
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
            }, timeout);

            return bool.Parse(result);
        }

        /// <summary>
        /// Prompts the user to add a new network to their Scatter.
        /// </summary>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<bool> SuggestNetwork(int? timeout = null)
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
            }, timeout);

            return bool.Parse(result);
        }

        //TODO check
        /// <summary>
        /// Request transfer of funds.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <param name="options"></param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<object> RequestTransfer(string to, string amount, object options = null, int? timeout = null)
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
            }, timeout);

            return result;
        }

        /// <summary>
        /// Request transaction signature
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<SignaturesResult> RequestSignature(object payload, int? timeout = null)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest<object, SignaturesResult>(new Request<object>()
            {
                type = "requestSignature",
                payload = payload
            }, timeout);

            return result;
        }

        /// <summary>
        /// Add token to wallet
        /// </summary>
        /// <param name="token"></param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<bool> AddToken(Token token, int? timeout = null)
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
            }, timeout);

            return bool.Parse(result);
        }

        /// <summary>
        /// Update identity information
        /// </summary>
        /// <param name="name">identity name</param>
        /// <param name="kyc">kyc information</param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<string> UpdateIdentity(string name, string kyc = null, int? timeout = null)
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
            }, timeout);

            return result;
        }

        //TODO test on new branch
        public async Task<string> GetEncryptionKey(string fromPublicKey, string toPublicKey, UInt64 nonce, int? timeout = null)
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
            }, timeout);

            return result;
        }

        /// <summary>
        /// Register listener for scatter event type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public void On(string type, Action<object> callback)
        {
            SocketService.On(type, callback);
        }

        /// <summary>
        /// Remove listener by event type
        /// </summary>
        /// <param name="type">event type</param>
        public void Off(string type)
        {
            SocketService.Off(type);
        }

        /// <summary>
        /// Remove listener by event type and position
        /// </summary>
        /// <param name="type">event type</param>
        /// <param name="index">position</param>
        public void Off(string type, int index)
        {
            SocketService.Off(type, index);
        }

        /// <summary>
        /// Remove listener by callback instance
        /// </summary>
        /// <param name="callback"></param>
        public void Off(Action<object> callback)
        {
            SocketService.Off(callback);
        }

        /// <summary>
        /// remove listner by event type and callback instance
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
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
