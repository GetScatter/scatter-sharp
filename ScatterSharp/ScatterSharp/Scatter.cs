using ScatterSharp.Core;
using ScatterSharp.Core.Storage;
using SocketIOSharp.Core;

namespace ScatterSharp
{
    public class Scatter : ScatterBase
    {
        public Scatter(ScatterConfigurator config) :
            base(config, new SocketService(config.StorageProvider ?? new MemoryStorageProvider(), new SocketIOConfigurator()
            {
                Namespace = "scatter",
                Proxy = new Proxy()
                {
                    Url = "http://127.0.0.1:8888"
                }
            }, config.AppName))
        {
        }
    }

}