using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Storage
{
    public class MemoryStorageProvider : IStorageProvider
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

        public bool SetAppkey(string appkey)
        {
            Appkey = appkey;
            return true;
        }

        public bool SetNonce(string nonce)
        {
            Nonce = nonce;
            return true;
        }
    }
}
