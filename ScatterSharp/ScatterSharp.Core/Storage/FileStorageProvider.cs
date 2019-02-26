using ScatterSharp.Core.Interfaces;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ScatterSharp.Core.Storage
{
    public class FileStorageProvider : IAppStorageProvider
    {
        [Serializable]
        public class AppData
        {
            public string Appkey;
            public string Nonce;
        }

        private string FilePath { get; set; }
        private AppData Data { get; set; }

        /// <summary>
        /// Construct file based storage implementation
        /// </summary>
        /// <param name="filePath">File path to create/update file</param>
        public FileStorageProvider(string filePath)
        {
            FilePath = filePath;
            Load();
        }

        /// <summary>
        /// Get Nonce
        /// </summary>
        /// <returns></returns>
        public string GetNonce()
        {
            return Data.Nonce;
        }

        /// <summary>
        /// Set nonce as hex string
        /// </summary>
        /// <param name="nonce"></param>
        public void SetNonce(string nonce)
        {
            Data.Nonce = nonce;
        }

        /// <summary>
        /// Get app key
        /// </summary>
        /// <returns></returns>
        public string GetAppkey()
        {
            return Data.Appkey;
        }

        /// <summary>
        /// Set appkey
        /// </summary>
        /// <param name="appkey"></param>
        public void SetAppkey(string appkey)
        {
            Data.Appkey = appkey;
        }

        /// <summary>
        /// Save all information to storage
        /// </summary>
        public void Save()
        {
            var bf = new BinaryFormatter();
            var fs = File.Create(FilePath);
            bf.Serialize(fs, Data);
            fs.Close();
        }

        /// <summary>
        /// Load all information from storage
        /// </summary>
        public void Load()
        {
            if (!File.Exists(FilePath))
            {
                Data = new AppData();
                return;
            }

            var bf = new BinaryFormatter();
            var fs = File.Open(FilePath, FileMode.Open);
            Data = (AppData)bf.Deserialize(fs);

            fs.Close();
        }
    }
}
