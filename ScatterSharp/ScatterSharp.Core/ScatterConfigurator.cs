using ScatterSharp.Core.Api;
using ScatterSharp.Core.Interfaces;
using SocketIOSharp.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Core
{
    /// <summary>
    /// Aggregates all properties to configure Scatter client
    /// </summary>
    public class ScatterConfigurator
    {
        /// <summary>
        /// App name (origin) for identifying the application
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// Network used in the client
        /// </summary>
        public Network Network { get; set; } 
        /// <summary>
        /// Storage implemantation to save appName and nonce
        /// </summary>
        public IAppStorageProvider StorageProvider { get; set; }
        /// <summary>
        /// Proxy to route traffic (optional)
        /// </summary>
        public Proxy Proxy { get; set; }
        /// <summary>
        /// Default Timeout for all requests
        /// </summary>
        public int DefaultTimeout = 60000;
    }
}
