namespace ScatterSharp.Core.Interfaces
{
    public interface IAppStorageProvider
    {
        /// <summary>
        /// Get Nonce
        /// </summary>
        /// <returns></returns>
        string GetNonce();

        /// <summary>
        /// Set nonce as hex string
        /// </summary>
        /// <param name="nonce"></param>
        void SetNonce(string nonce);

        /// <summary>
        /// Get app key
        /// </summary>
        /// <returns></returns>
        string GetAppkey();

        /// <summary>
        /// Set appkey
        /// </summary>
        /// <param name="appkey"></param>
        void SetAppkey(string appkey);

        /// <summary>
        /// Save all information to storage
        /// </summary>
        void Save();

        /// <summary>
        /// Load all information from storage
        /// </summary>
        void Load();
    }
}
