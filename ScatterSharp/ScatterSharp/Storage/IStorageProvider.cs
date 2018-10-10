using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Storage
{
    public interface IAppStorageProvider
    {
        string GetNonce();
        void SetNonce(string nonce);
        string GetAppkey();
        void SetAppkey(string appkey);
        void Save();
        void Load();
    }
}
