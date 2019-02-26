using ScatterSharp.Core;
using ScatterSharp.Core.Storage;
using SocketIOSharp.Core;

namespace ScatterSharp
{
    /// <summary>
    /// Scatter client using generic socket service
    /// </summary>
    public class Scatter : ScatterBase
    {
        /// <summary>
        /// Constructor for scatter client with init configuration
        /// </summary>
        /// <param name="config">Configuration object</param>
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