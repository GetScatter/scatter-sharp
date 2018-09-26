using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScatterSharp
{
    public class Scatter : IDisposable
    {
        private ClientWebSocket Socket { get; set; }
        private readonly string WSURI = "ws://{0}/socket.io/?EIO=3&transport=websocket";

        public Scatter()
        {
            Socket = new ClientWebSocket();
        }

        public async Task Connect(string host, CancellationToken cancellationToken)
        {
            if (Socket.State != WebSocketState.Open && Socket.State != WebSocketState.Connecting)
            {
                await Socket.ConnectAsync(new Uri(string.Format(WSURI, host)), cancellationToken);
            }

            if (Socket.State == WebSocketState.Open)
                await Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("40/scatter")), WebSocketMessageType.Text, true, cancellationToken);
            else
                throw new Exception("Socket closed.");
        }

        private async Task Receive()
        {
            byte[] frame = new byte[4096];
            ArraySegment<byte> segment = new ArraySegment<byte>(frame, 0, frame.Length);
            byte[] messageBytes = null;

            while (Socket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult response;

                using (var ms = new MemoryStream())
                {
                    do
                    {
                        response = await Socket.ReceiveAsync(segment, CancellationToken.None);
                        ms.Write(segment.Array, segment.Offset, response.Count);
                    }
                    while (response.EndOfMessage);

                    messageBytes = ms.ToArray();
                }

                if (response.MessageType == WebSocketMessageType.Close)
                {
                    await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close response received", CancellationToken.None);
                }

            }
        }

        public void Dispose()
        {
            Socket.Dispose();
        }
    }
}
