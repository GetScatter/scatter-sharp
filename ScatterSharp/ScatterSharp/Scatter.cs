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
                Proxy = config.Proxy
            }, config.AppName))
        {
        }
    }

}