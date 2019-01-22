namespace ScatterSharp.Core.Interfaces
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
