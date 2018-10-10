using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ScatterSharp.Storage
{
    public class FileStorageProvider : IAppStorageProvider
    {
        public class AppData
        {
            public string Appkey;
            public string Nonce;
        }

        private string FilePath { get; set; }
        private AppData Data { get; set; }

        public FileStorageProvider(string filePath)
        {
            FilePath = filePath;
            Load();
        }

        public string GetAppkey()
        {
            return Data.Appkey;
        }

        public string GetNonce()
        {
            return Data.Nonce;
        }

        public void SetAppkey(string appkey)
        {
            Data.Appkey = appkey;
        }

        public void SetNonce(string nonce)
        {
            Data.Nonce = nonce;
        }

        public void Save()
        {
            var bf = new BinaryFormatter();
            var fs = File.Create(FilePath);
            bf.Serialize(fs, Data);
            fs.Close();
        }

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
