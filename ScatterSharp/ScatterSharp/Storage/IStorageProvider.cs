using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Storage
{
    public interface IStorageProvider
    {
        string GetNonce();
        bool SetNonce(string nonce);
        string GetAppkey();
        bool SetAppkey(string appkey);
    }
}
