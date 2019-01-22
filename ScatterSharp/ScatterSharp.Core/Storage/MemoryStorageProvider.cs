using ScatterSharp.Core.Interfaces;

namespace ScatterSharp.Core.Storage
{
    public class MemoryStorageProvider : IAppStorageProvider
    {
        private string Appkey { get; set; }
        private string Nonce { get; set; }

        public MemoryStorageProvider()
        {
        }

        public string GetAppkey()
        {
            return Appkey;
        }

        public string GetNonce()
        {
            return Nonce;
        }

        public void SetAppkey(string appkey)
        {
            Appkey = appkey;
        }

        public void SetNonce(string nonce)
        {
            Nonce = nonce;
        }

        public void Save()
        {
        }

        public void Load()
        {
        }
    }
}
