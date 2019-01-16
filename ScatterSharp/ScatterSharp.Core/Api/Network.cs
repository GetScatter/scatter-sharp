namespace ScatterSharp.Core.Api
{
    public class Network
    {
        public string name;
        public string blockchain;
        public string host;
        public int    port;
        public string protocol;
        public string chainId;

        public Network()
        {
            protocol = "https";
        }
    }
}