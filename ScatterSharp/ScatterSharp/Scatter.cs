using ScatterSharp.Core;
using ScatterSharp.Core.Api;
using ScatterSharp.Core.Interfaces;
using ScatterSharp.Core.Storage;
using System;
using System.Threading.Tasks;

namespace ScatterSharp
{
    public class Scatter : ScatterBase
    {
        private ISocketService SocketService { get; set; }

        public Scatter(string appName, Network network, IAppStorageProvider storageProvider = null) :
            base(appName, network, new SocketService(storageProvider ?? new MemoryStorageProvider(), appName))
        {
        }
    }

}
