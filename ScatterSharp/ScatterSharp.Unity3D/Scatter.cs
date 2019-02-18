using ScatterSharp.Core;
using ScatterSharp.Core.Storage;
using SocketIOSharp.Core;
using UnityEngine;

namespace ScatterSharp.Unity3D
{
    public class Scatter : ScatterBase
    {
        public Scatter(ScatterConfigurator config, MonoBehaviour scriptInstance = null) :
            base(config, new SocketService(config.StorageProvider ?? new MemoryStorageProvider(), new SocketIOConfigurator()
            {
                Namespace = "scatter",
                Proxy = new Proxy()
                {
                    Url = "http://127.0.0.1:8888"
                }
            }, config.AppName, 60000, scriptInstance))
        {
        }
    }

}
