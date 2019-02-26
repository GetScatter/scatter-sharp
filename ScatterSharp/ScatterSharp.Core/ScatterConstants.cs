using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Core
{
    public class ScatterConstants
    {
        /// <summary>
        /// Websocket URI string format
        /// </summary>
        public static readonly string WSURI = "{0}://{1}:{2}/socket.io/?EIO=3&transport=websocket";

        /// <summary>
        /// Web socket SSL information
        /// </summary>
        public static readonly string WSS_PROTOCOL = "wss";
        public static readonly string WSS_HOST = "local.get-scatter.com";
        public static readonly string WSS_PORT = "50006";

        /// <summary>
        /// Web socket information
        /// </summary>
        public static readonly string WS_PROTOCOL = "ws";
        public static readonly string WS_HOST = "127.0.0.1";
        public static readonly string WS_PORT = "50005";

        /// <summary>
        /// Interval in secs to check for open thats that timeout
        /// </summary>
        public static readonly int OPEN_TASK_CHECK_INTERVAL_SECS = 1;

        /// <summary>
        /// Max number of tasks to check per cycle
        /// </summary>
        public static readonly int OPEN_TASK_NR_CHECK = 10;


        public class Blockchains
        {
            public static readonly string EOSIO = "eos";
            public static readonly string ETH = "eth";
            public static readonly string TRX = "trx";
        };

        public class Events
        {
            public static readonly string Disconnected = "dced";
            public static readonly string LoggedOut = "logout";
        };
    }
}
