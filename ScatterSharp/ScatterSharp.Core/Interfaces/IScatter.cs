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
        /// <returns></returns>
        Task Connect();

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
        /// <returns></returns>
        Task<string> GetVersion();

        /// <summary>
        /// Get scatter identity (login)
        /// </summary>
        /// <param name="requiredFields">Optional required fields</param>
        /// <returns></returns>
        Task<Identity> GetIdentity(IdentityRequiredFields requiredFields = null);

        /// <summary>
        /// Get scatter identity from available permissions
        /// </summary>
        /// <returns></returns>
        Task<Identity> GetIdentityFromPermissions();

        /// <summary>
        /// Forget loggedin identity 
        /// </summary>
        /// <returns></returns>
        Task<bool> ForgetIdentity();

        /// <summary>
        /// Authenticate by signing origin (appName) or custom data with custom publicKey
        /// </summary>
        /// <param name="nonce">entropy nonce</param>
        /// <param name="data">custom data</param>
        /// <param name="publicKey">custom publickey</param>
        /// <returns></returns>
        Task<string> Authenticate(string nonce, string data = null, string publicKey = null);

        /// <summary>
        /// Sign arbitrary data with the constraint of max 12 words
        /// </summary>
        /// <param name="publicKey">publickey to request the private key signature</param>
        /// <param name="data">data to sign</param>
        /// <param name="whatfor">Optional reason for signature</param>
        /// <param name="isHash">is data a sha256 hash</param>
        /// <returns></returns>
        Task<string> GetArbitrarySignature(string publicKey, string data, string whatfor = "", bool isHash = false);

        /// <summary>
        /// Ask user to provide a publickey inside the wallet
        /// </summary>
        /// <param name="blockchain"></param>
        /// <returns></returns>
        Task<string> GetPublicKey(string blockchain);

        /// <summary>
        /// link account to the given network
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task<bool> LinkAccount(LinkAccount account);

        /// <summary>
        /// Does the account exists for given network
        /// </summary>
        /// <returns></returns>
        Task<bool> HasAccountFor();

        /// <summary>
        /// Suggest network change
        /// </summary>
        /// <returns></returns>
        Task<bool> SuggestNetwork();

        /// <summary>
        /// Request transfer of funds
        /// </summary>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<object> RequestTransfer(string to, string amount, object options = null);

        /// <summary>
        /// Request transaction signature
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        Task<SignaturesResult> RequestSignature(object payload);

        /// <summary>
        /// Add token to wallet
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> AddToken(Token token);

        Task<string> GetEncryptionKey(string fromPublicKey, string toPublicKey, UInt64 nonce);

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
