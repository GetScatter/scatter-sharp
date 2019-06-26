using ScatterSharp.Core.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScatterSharp.Core.Interfaces
{
    public interface IScatter : IDisposable
    {
        /// <summary>
        /// Connect to scatter
        /// </summary>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task Connect(int? timeout = null);

        /// <summary>
        /// Get configured network
        /// </summary>
        /// <returns></returns>
        Network GetNetwork();

        /// <summary>
        /// Get configured app name
        /// </summary>
        /// <returns></returns>
        string GetAppName();

        /// <summary>
        /// Get Scatter version
        /// </summary>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<string> GetVersion(int? timeout = null);

        /// <summary>
        /// Prompts the users for an Identity if there is no permission, otherwise returns the permission without a prompt based on origin.
        /// </summary>
        /// <param name="requiredFields">Optional required fields</param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<Identity> GetIdentity(IdentityRequiredFields requiredFields = null, int? timeout = null);

        /// <summary>
        /// Prompts the users for an Identity if there is no permission, otherwise returns the permission without a prompt based on origin.
        /// </summary>
        /// <param name="requiredFields">Optional required fields for multiple networks at once</param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<Identity> LoginAll(IdentityRequiredFields requiredFields = null, int? timeout = null);

        /// <summary>
        /// Checks if an Identity has permissions and return the identity based on origin.
        /// </summary>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<Identity> GetIdentityFromPermissions(int? timeout = null);

        /// <summary>
        /// Get authenticated user account
        /// </summary>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<byte[]> GetAvatar(int? timeout = null);

        /// <summary>
        /// Removes the identity permission for an origin from the user's Scatter, effectively logging them out.
        /// </summary>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<bool> ForgetIdentity(int? timeout = null);

        /// <summary>
        /// Sign origin (appName) with the Identity's private key. Or custom data with custom publicKey
        /// </summary>
        /// <param name="nonce">entropy nonce</param>
        /// <param name="data">custom data</param>
        /// <param name="publicKey">custom publickey</param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<string> Authenticate(string nonce, string data = null, string publicKey = null, int? timeout = null);

        /// <summary>
        /// Sign arbitrary data with the constraint of max 12 words
        /// </summary>
        /// <param name="publicKey">publickey to request the private key signature</param>
        /// <param name="data">data to sign</param>
        /// <param name="whatfor">Optional reason for signature</param>
        /// <param name="isHash">is data a sha256 hash</param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<string> GetArbitrarySignature(string publicKey, string data, string whatfor = "", bool isHash = false, int? timeout = null);

        /// <summary>
        /// Allows apps to request that the user provide a user-selected Public Key to the app. ( ONBOARDING HELPER )
        /// </summary>
        /// <param name="blockchain"></param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<string> GetPublicKey(string blockchain, int? timeout = null);

        /// <summary>
        /// Allows the app to suggest that the user link new accounts on top of public keys ( ONBOARDING HELPER )
        /// </summary>
        /// <param name="account"></param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<bool> LinkAccount(LinkAccount account, int? timeout = null);

        /// <summary>
        /// Allows dapps to see if a user has an account for a specific blockchain. DOES NOT PROMPT and does not return an actual account, just a boolean.
        /// </summary>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<bool> HasAccountFor(int? timeout = null);

        /// <summary>
        /// Prompts the user to add a new network to their Scatter.
        /// </summary>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<bool> SuggestNetwork(int? timeout = null);

        /// <summary>
        /// Request transfer of funds.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <param name="options"></param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<object> RequestTransfer(string to, string amount, object options = null, int? timeout = null);

        /// <summary>
        /// Request transaction signature
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<SignaturesResult> RequestSignature(object payload, int? timeout = null);

        /// <summary>
        /// Add token to wallet
        /// </summary>
        /// <param name="token"></param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<bool> AddToken(Token token, int? timeout = null);

        /// <summary>
        /// Update identity information
        /// </summary>
        /// <param name="name">identity name</param>
        /// <param name="kyc">kyc information</param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<string> UpdateIdentity(string name, string kyc = null, int? timeout = null);

        Task<string> GetEncryptionKey(string fromPublicKey, string toPublicKey, UInt64 nonce, int? timeout = null);

        /// <summary>
        /// Register listener for scatter event type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        void On(string type, Action<object> callback);

        /// <summary>
        /// Remove listener by event type
        /// </summary>
        /// <param name="type">event type</param>
        void Off(string type);

        /// <summary>
        /// Remove listener by event type and position
        /// </summary>
        /// <param name="type">event type</param>
        /// <param name="index">position</param>
        void Off(string type, int index);

        /// <summary>
        /// Remove listener by callback instance
        /// </summary>
        /// <param name="callback"></param>
        void Off(Action<object> callback);

        /// <summary>
        /// remove listner by event type and callback instance
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        void Off(string type, Action<object> callback);
    }
}
