using Newtonsoft.Json.Linq;
using ScatterSharp.Core;
using ScatterSharp.Core.Api;
using ScatterSharp.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace ScatterSharp.Unity3D
{
    public class Scatter : IScatter
    {
        private SocketService SocketService { get; set; }
        private IAppStorageProvider StorageProvider { get; set; }

        public string AppName { get; set; }
        public Network Network { get; set; }
        public Identity Identity { get; set; }

        public Scatter(string appName, Network network, IAppStorageProvider storageProvider = null)
        {
            StorageProvider = storageProvider;
            AppName = appName;
            Network = network;
        }

        public void Dispose()
        {
            SocketService.Dispose();
        }

        public async Task Connect()
        {
            try
            {
                //Try connect with wss connection
                SocketService = new SocketService(StorageProvider, AppName);
                Uri wssURI = new Uri(string.Format(ScatterConstants.WSURI, 
                                                   ScatterConstants.WSS_PROTOCOL, 
                                                   ScatterConstants.WSS_HOST, 
                                                   ScatterConstants.WSS_PROTOCOL));

                await SocketService.Link(wssURI);
            }
            catch(Exception)
            {
                //try normal ws connection
                SocketService.Dispose();
                SocketService = new SocketService(StorageProvider, AppName);
                Uri wsURI = new Uri(string.Format(ScatterConstants.WSURI,
                                                   ScatterConstants.WS_PROTOCOL,
                                                   ScatterConstants.WS_HOST,
                                                   ScatterConstants.WS_PROTOCOL));

                await SocketService.Link(wsURI);
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
            var result = await SocketService.SendApiRequest(new Request()
            {
                type = "getVersion",
                payload = new { origin = AppName }
            });

            ThrowOnApiError(result);

            return result.ToObject<string>();
        }

        public async Task<Identity> GetIdentity(IdentityRequiredFields requiredFields)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                type = "getOrRequestIdentity",
                payload = new { fields = requiredFields, origin = AppName }
            });

            ThrowOnApiError(result);
            
            Identity = result.ToObject<Identity>();

            return Identity;
        }

        public async Task<Identity> GetIdentityFromPermissions()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                type = "identityFromPermissions",
                payload = new { origin = AppName }
            });

            ThrowOnApiError(result);

            if(result.Type == JTokenType.Object)
                Identity = result.ToObject<Identity>();

            return Identity;
        }

        public async Task<bool> ForgetIdentity()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                type = "forgetIdentity",
                payload = new { origin = AppName }
            });

            ThrowOnApiError(result);

            Identity = null;
            return result.ToObject<bool>();
        }

        public async Task<string> Authenticate(string nonce, string data = null, string publicKey = null)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                type = "authenticate",
                payload = new { nonce, data, publicKey, origin = AppName }
            });

            ThrowOnApiError(result);

            return result.ToObject<string>();
        }

        public async Task<string> GetArbitrarySignature(string publicKey, string data, string whatfor = "", bool isHash = false)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                type = "requestArbitrarySignature",
                payload = new { publicKey, data, whatfor, isHash, origin = AppName }
            });

            ThrowOnApiError(result);

            return result.ToObject<string>();
        }

        public async Task<string> GetPublicKey(string blockchain)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                type = "getPublicKey",
                payload = new { blockchain, origin = AppName }
            });

            ThrowOnApiError(result);

            return result.ToObject<string>();
        }

        public async Task<bool> LinkAccount(LinkAccount account)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                type = "linkAccount",
                payload = new { account, network = Network, origin = AppName }
            });

            ThrowOnApiError(result);

            return result.ToObject<bool>();
        }

        public async Task<bool> HasAccountFor()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                type = "hasAccountFor",
                payload = new { network = Network, origin = AppName }
            });

            ThrowOnApiError(result);

            return result.ToObject<bool>();
        }

        public async Task<bool> SuggestNetwork()
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                type = "requestAddNetwork",
                payload = new { network = Network, origin = AppName }
            });

            ThrowOnApiError(result);

            return result.ToObject<bool>();
        }

        //TODO check
        public async Task<object> RequestTransfer(string to, string amount, object options = null)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                type = "requestTransfer",
                payload = new { network = Network, to, amount, options, origin = AppName }
            });

            ThrowOnApiError(result);

            return result;
        }

        public async Task<SignaturesResult> RequestSignature(object payload)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                type = "requestSignature",
                payload = payload
            });

            ThrowOnApiError(result);

            return result.ToObject<SignaturesResult>();
        }

        public async Task<bool> AddToken(Token token)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
            {
                type = "addToken",
                payload = new { token, network = Network, origin = AppName }
            });

            ThrowOnApiError(result);

            return result.ToObject<bool>();
        }

        //TODO test on new branch
        public async Task<string> GetEncryptionKey(string fromPublicKey, string toPublicKey, UInt64 nonce)
        {
            ThrowNoAuth();

            var result = await SocketService.SendApiRequest(new Request()
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

            ThrowOnApiError(result);

            return result.ToObject<string>();
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
                throw new Exception(apiError.message);
        }

        #endregion
    }

}
