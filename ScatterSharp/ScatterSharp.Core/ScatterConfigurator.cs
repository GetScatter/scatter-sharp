using ScatterSharp.Core.Api;
using ScatterSharp.Core.Interfaces;
using SocketIOSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Core
{
    public class ScatterConfigurator
    {
        public string AppName { get; set; }
        public Network Network { get; set; } 
        public IAppStorageProvider StorageProvider { get; set; }
        public Proxy Proxy { get; set; }
    }
}
