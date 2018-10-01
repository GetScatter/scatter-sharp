using Cryptography.ECDSA;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScatterSharp.Api;
using ScatterSharp.Helpers;
using ScatterSharp.Storage;
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
        private IStorageProvider StorageProvider { get; set; }
        private string AppName { get; set; }

        private ClientWebSocket Socket { get; set; }

        TaskCompletionSource<bool> PairOpenTask { get; set; }
        private Dictionary<string, TaskCompletionSource<object>> OpenTasks { get; set; }
        private Task ReceiverTask { get; set; }

        public SocketService(IStorageProvider storageProvider, string appName, int timeout = 60000)
        {
            Socket = new ClientWebSocket();
            OpenTasks = new Dictionary<string, TaskCompletionSource<object>>();
            StorageProvider = storageProvider;
            AppName = appName;

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
                var receiverTask = Receive();
                await Pair(true);
            }  
            else
                throw new Exception("Socket closed.");

        }

        public async Task Pair(bool passthrough = false)
        {
            PairOpenTask = new TaskCompletionSource<bool>();

            await Send("pair", new
            {
                data = new
                {
                    appkey = StorageProvider.GetAppkey(),
                    passthrough,
                    origin = AppName
                },
                plugin = AppName
            });

            await PairOpenTask.Task;
        }

        public async Task<object> SendApiRequest(ApiRequest request)
        {
            if (request.Type == "identityFromPermissions" && !Paired)
                return Task.FromResult(false);

            await Pair();

            if (!Paired)
                return Task.FromResult(new { code = "not_paired", message = "The user did not allow this app to connect to their Scatter" });

            var tcs = new TaskCompletionSource<object>();

            request.Id = UtilsHelper.RandomNumber();
            request.Appkey = StorageProvider.GetAppkey();
            request.Nonce = StorageProvider.GetNonce() ?? "0";

            var nextNonce = UtilsHelper.RandomNumber();
            request.NextNonce = UtilsHelper.GenerateNextNonce();
            StorageProvider.SetNonce(request.NextNonce);

            OpenTasks.Add(request.Id, tcs);
            await Send("api", new { data = request, plugin = AppName });

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

                string jsonStr = null;
                using (var sr = new StreamReader(ms))
                {
                    jsonStr = sr.ReadToEnd();
                }
                ms.Dispose();

                var jArr = JArray.Parse(jsonStr);
  
                if (jArr.Count == 0)
                    continue;

                string type = jArr[0].ToObject<string>();

                switch (type)
                {
                    case "paired":
                        if(jArr.Count == 2)
                            HandlePairedResponse(jArr[1].ToObject<bool>());
                        break;
                    case "rekey":
                        HandleRekeyResponse();
                        break;
                    case "api":
                        if (jArr.Count == 2)
                            HandleApiResponse(jArr[1].ToObject<ApiMessage>());
                        break;
                }
            }
        }

        private void HandleApiResponse(ApiMessage data)
        {
            if (data == null)
                return;

            if (!OpenTasks.TryGetValue(data.Id, out TaskCompletionSource<object> openTask))
                return;

            openTask.SetResult(data.Result);

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
            Send("rekeyed", new
            {
                plugin = AppName,
                data = new
                {
                    origin = AppName,
                    appkey = StorageProvider.GetAppkey()
                }
            });
        }

        private void HandlePairedResponse(bool paired)
        {
            Paired = paired;

            if (Paired)
            {
                var storedAppKey = StorageProvider.GetAppkey();

                string hashed = storedAppKey.StartsWith("appkey:") ?
                    UtilsHelper.ByteArrayToHexString(Sha256Manager.GetHash(Encoding.UTF8.GetBytes(storedAppKey))) :
                    storedAppKey;

                if (string.IsNullOrWhiteSpace(storedAppKey) ||
                    storedAppKey != hashed)
                {
                    StorageProvider.SetAppkey(hashed);
                }
            }

            if(PairOpenTask != null)
            {
                PairOpenTask.SetResult(Paired);
            }
        }

        private void GenerateNewAppKey()
        {
            StorageProvider.SetAppkey("appkey:" + UtilsHelper.RandomNumber());
        }

        #endregion
    }
}
