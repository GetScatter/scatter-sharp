using ScatterSharp.Core;
using ScatterSharp.Core.Storage;
using SocketIOSharp.Core;
using UnityEngine;

namespace ScatterSharp.Unity3D
{
    /// <summary>
    /// Scatter client using socket service unity3d implementation
    /// </summary>
    public class Scatter : ScatterBase
    {
        /// <summary>
        /// Constructor for scatter client with init configuration
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <param name="scriptInstance">script instance for using coroutines</param>
        public Scatter(ScatterConfigurator config, MonoBehaviour scriptInstance = null) :
            base(config, new SocketService(config.StorageProvider ?? new MemoryStorageProvider(), new SocketIOConfigurator()
            {
                Namespace = "scatter",
                Proxy = config.Proxy
            }, config.AppName, 60000, scriptInstance))
        {
        }
    }

}
