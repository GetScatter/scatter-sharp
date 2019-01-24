using ScatterSharp.Core;
using ScatterSharp.Core.Storage;

namespace ScatterSharp
{
    public class Scatter : ScatterBase
    {
        public Scatter(ScatterConfigurator config) :
            base(config, new SocketService(config.StorageProvider ?? new MemoryStorageProvider(), config.AppName))
        {
        }
    }

}