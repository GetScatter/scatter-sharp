using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Core
{
    public class ScatterConstants
    {
        public static readonly string WSURI = "{0}://{1}:{2}/socket.io/?EIO=3&transport=websocket";

        public static readonly string WSS_PROTOCOL = "wss";
        public static readonly string WSS_HOST = "local.get-scatter.com";
        public static readonly string WSS_PORT = "50006";

        public static readonly string WS_PROTOCOL = "ws";
        public static readonly string WS_HOST = "127.0.0.1";
        public static readonly string WS_PORT = "50005";

        public class Blockchains
        {
            public static readonly string EOSIO = "eos";
            public static readonly string ETH = "eth";
            public static readonly string TRX = "trx";
        };
    }
}
