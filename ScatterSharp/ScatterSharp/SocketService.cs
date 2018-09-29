using Cryptography.ECDSA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScatterSharp
{
    public class SocketService : IDisposable
    {
        private readonly string SCATTER_API_PREAMBLE = "42/scatter";

        private bool Paired { get; set; }
        private string StoredNonce { get; set; }
        private string StoredAppkey { get; set; }

        private ClientWebSocket Socket { get; set; }

        TaskCompletionSource<bool> PairOpenTask { get; set; }
        private Dictionary<string, TaskCompletionSource<object>> OpenTasks { get; set; }
        private Task ReceiverTask { get; set; }

        public SocketService(int timeout = 60000)
        {
            Socket = new ClientWebSocket();
            OpenTasks = new Dictionary<string, TaskCompletionSource<object>>();
            GenerateNewAppKey();
        }

        public void Dispose()
        {
            Socket.Dispose();
        }

        public async Task Link(Uri uri, CancellationToken? cancellationToken)
        {
            if (Socket.State != WebSocketState.Open && Socket.State != WebSocketState.Connecting)
            {
                await Socket.ConnectAsync(uri, cancellationToken ?? CancellationToken.None);
            }

            if (Socket.State == WebSocketState.Open)
            {
                await Send();
                await Pair(true);
            }  
            else
                throw new Exception("Socket closed.");

            var receiverTask = Receive();

            //this.identity = await this.getIdentityFromPermissions();
        }

        public Task Pair(bool passthrough = false)
        {
            PairOpenTask = new TaskCompletionSource<bool>();
            return Send("pair", new { data = new { appkey = StoredAppkey, passthrough, origin = "ABC"}, plugin = "ABC" });
        }

        public async Task<object> SendApiRequest(ScatterApiRequest request)
        {

            //    if (request.type === 'identityFromPermissions' && !paired) return resolve(false);

            //    pair().then(() => {
            //        if (!paired) return reject({ code: 'not_paired', message: 'The user did not allow this app to connect to their Scatter'});

            //    // Request ID used for resolving promises
            //    request.id = random();

            //    // Set Application Key
            //    request.appkey = appkey;

            //    // Nonce used to authenticate this request
            //    request.nonce = StorageService.getNonce() || 0;
            //    // Next nonce used to authenticate the next request
            //    const nextNonce = random();
            //    request.nextNonce = sha256(nextNonce);
            //    StorageService.setNonce(nextNonce);

            //    openRequests.push(Object.assign(request, { resolve, reject}));
            //    send('api', { data: request, plugin})
            //    })

            await Pair();

            var tcs = new TaskCompletionSource<object>();

            request.Id = RandomNumber();
            request.Appkey = StoredAppkey;
            request.Nonce = StoredNonce ?? "0";
            request.NextNonce = RandomNumber();
            StoredNonce = request.NextNonce;

            OpenTasks.Add(request.Id, tcs);
            await Send("api", new { data = request, plugin = "ABC" });

            return await tcs.Task;
        }

        public Task Disconnect(CancellationToken? cancellationToken = null)
        {
            return Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close socket received", cancellationToken ?? CancellationToken.None);
        }

        public bool IsConnected()
        {
            return Socket.State == WebSocketState.Open;
        }

        public bool IsPaired()
        {
            return Paired;
        }

        #region Utils
        private Task Send(string type = null, object data = null)
        {
            if (string.IsNullOrWhiteSpace(type) && data == null)
            {
                return Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("40/scatter")), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                var arraySeg = new ArraySegment<byte>(
                    Encoding.UTF8.GetBytes(string.Format("42/scatter,{0}", JsonConvert.SerializeObject(new List<object>()
                    {
                        type,
                        data
                    })))
                );
                return Socket.SendAsync(arraySeg, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private async Task Receive()
        {
            byte[] frame = new byte[4096];
            byte[] preamble = new byte[SCATTER_API_PREAMBLE.Length];
            ArraySegment<byte> segment = new ArraySegment<byte>(frame, 0, frame.Length);
            List<object> apiResponse = null;

            while (Socket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;
                MemoryStream ms = null;

                do
                {
                    result = await Socket.ReceiveAsync(segment, CancellationToken.None);

                    if (ms == null)
                        ms = new MemoryStream();

                    ms.Write(segment.Array, segment.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    ms.Dispose();
                    await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close response received", CancellationToken.None);
                    continue;
                }

                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(preamble, 0, preamble.Length);

                // Disregarding Handshaking/Upgrading
                if (Encoding.UTF8.GetString(preamble) != SCATTER_API_PREAMBLE)
                {
                    ms.Dispose();
                    continue;
                }

                //skip , from preamble
                ms.Seek(ms.Position + 1, SeekOrigin.Begin);

                using (var sr = new StreamReader(ms))
                using (var jtr = new JsonTextReader(sr))
                {
                    apiResponse = JsonSerializer.Create().Deserialize<List<object>>(jtr);
                }

                ms.Dispose();

                if (apiResponse.Count == 0)
                    continue;

                var type = apiResponse[0] as string;
                var data = apiResponse.Count == 2 ? apiResponse[1] : null;

                switch (type)
                {
                    case "paired":
                        HandlePairedResponse(data);
                        break;
                    case "rekey":
                        HandleRekeyResponse();
                        break;
                    case "api":
                        HandleApiResponse(data);
                        break;
                }
            }
        }

        private void HandleApiResponse(object data)
        {
            


            //const openRequest = openRequests.find(x => x.id === response.id);
            //if (!openRequest) return;

            //openRequests = openRequests.filter(x => x.id !== response.id);

            //const isErrorResponse = typeof response.result === 'object'
            //    && response.result !== null
            //    && response.result.hasOwnProperty('isError');

            //if (isErrorResponse) openRequest.reject(response.result);
            //else openRequest.resolve(response.result);
        }

        private void HandleRekeyResponse()
        {
            GenerateNewAppKey();
            Send("rekeyed", new { plugin = "ABC", data = new { origin = "ABC", appkey = StoredAppkey } });
        }

        private void HandlePairedResponse(object data)
        {
            Paired = data != null && data is bool ? (bool)data : false;

            if (Paired)
            {
                string hashed = StoredAppkey.StartsWith("appkey:") ?
                    Encoding.UTF8.GetString(Sha256Manager.GetHash(Encoding.UTF8.GetBytes(StoredAppkey))) :
                    StoredAppkey;

                if (string.IsNullOrWhiteSpace(StoredAppkey) ||
                    StoredAppkey != hashed)
                {
                    StoredAppkey = hashed;
                }
            }

            if(PairOpenTask != null)
            {
                PairOpenTask.SetResult(Paired);
            }
        }

        private string RandomNumber()
        {
            var r = RandomNumberGenerator.Create();
            byte[] numberBytes = new byte[24];
            r.GetBytes(numberBytes);
            return Encoding.UTF8.GetString(numberBytes);
        }

        private void GenerateNewAppKey()
        {
            StoredAppkey = "appkey:" + RandomNumber();
        }

        #endregion
    }
}
