using ScatterSharp.Core.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScatterSharp.Core.Interfaces
{
    public interface IScatter : IDisposable
    {
        Task Connect();

        Network GetNetwork();
        string GetAppName();

        Task<string> GetVersion();
        Task<Identity> GetIdentity(IdentityRequiredFields requiredFields = null);
        Task<Identity> GetIdentityFromPermissions();
        Task<bool> ForgetIdentity();
        Task<string> Authenticate(string nonce, string data = null, string publicKey = null);
        Task<string> GetArbitrarySignature(string publicKey, string data, string whatfor = "", bool isHash = false);
        Task<string> GetPublicKey(string blockchain);
        Task<bool> LinkAccount(LinkAccount account);
        Task<bool> HasAccountFor();
        Task<bool> SuggestNetwork();
        Task<object> RequestTransfer(string to, string amount, object options = null);
        Task<SignaturesResult> RequestSignature(object payload);
        Task<bool> AddToken(Token token);
        Task<string> GetEncryptionKey(string fromPublicKey, string toPublicKey, UInt64 nonce);
        void On(string type, Action<object> callback);
        void Off(string type);
        void Off(string type, int index);
        void Off(Action<object> callback);
        void Off(string type, Action<object> callback);
    }
}
