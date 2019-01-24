using Cryptography.ECDSA;
using Newtonsoft.Json.Linq;
using ScatterSharp.Core.Api;
using ScatterSharp.Core.Helpers;
using ScatterSharp.Core.Interfaces;
using SocketIOSharp;
using SocketIOSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace ScatterSharp
{
    public class SocketService : ISocketService
    {
        private bool Paired { get; set; }
        private IAppStorageProvider StorageProvider { get; set; }
        private string AppName { get; set; }
        private int TimeoutMS { get; set; }

        private SocketIO SockIO { get; set; }

        TaskCompletionSource<bool> PairOpenTask { get; set; }
        private Dictionary<string, TaskCompletionSource<JToken>> OpenTasks { get; set; }
        private Dictionary<string, DateTime> OpenTaskTimes { get; set; }

        private Task TimoutTasksTask { get; set; }

        public SocketService(IAppStorageProvider storageProvider, string appName, int timeout = 60000)
        {
            SockIO = new SocketIO(new SocketIOConfigurator() {
                Namespace = "scatter",
                Proxy = new Proxy()
                {
                    Url = "http://127.0.0.1:8888"
                }
            });

            OpenTasks = new Dictionary<string, TaskCompletionSource<JToken>>();
            OpenTaskTimes = new Dictionary<string, DateTime>();

            StorageProvider = storageProvider;
            AppName = appName;
            TimeoutMS = timeout;
        }

        public void Dispose()
        {
            SockIO.Dispose();
            StorageProvider.Save();
        }

        public async Task<bool> Link(Uri uri)
        {
            if (SockIO.GetState() == WebSocketState.Open)
                return true;

            if (SockIO.GetState() != WebSocketState.Open && SockIO.GetState() != WebSocketState.Connecting)
            {
                await SockIO.ConnectAsync(uri);
            }

            if (SockIO.GetState() != WebSocketState.Open)
                return false;

            SockIO.On("paired", (args) =>
            {
                HandlePairedResponse(args.First().ToObject<bool?>());
            });

            SockIO.On("rekey", (args) =>
            {
                HandleRekeyResponse();
            });

            SockIO.On("api", (args) =>
            {
                HandleApiResponse(args.First());
            });

            TimoutTasksTask = Task.Run(() => TimeoutOpenTasksCheck());

            await Pair(true);
            return true;
        }

        public async Task Pair(bool passthrough = false)
        {
            PairOpenTask = new TaskCompletionSource<bool>();

            await SockIO.EmitAsync("pair", new
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

        public async Task<TReturn> SendApiRequest<TReturn>(Request request)
        {
            if (request.type == "identityFromPermissions" && !Paired)
                return default(TReturn);

            await Pair();

            if (!Paired)
                throw new Exception("The user did not allow this app to connect to their Scatter");

            var tcs = new TaskCompletionSource<JToken>();

            request.id = UtilsHelper.RandomNumber(24);
            request.appkey = StorageProvider.GetAppkey();
            request.nonce = StorageProvider.GetNonce() ?? "";

            var nextNonce = UtilsHelper.RandomNumberBytes();
            request.nextNonce = UtilsHelper.ByteArrayToHexString(Sha256Manager.GetHash(nextNonce));
            StorageProvider.SetNonce(UtilsHelper.ByteArrayToHexString(nextNonce));

            OpenTasks.Add(request.id, tcs);
            OpenTaskTimes.Add(request.id, DateTime.Now);

            await SockIO.EmitAsync("api", new { data = request, plugin = AppName });

            var result = await tcs.Task;

            ThrowOnApiError(result);

            return result.ToObject<TReturn>();
        }

        public Task Disconnect()
        {
            return SockIO.DisconnectAsync(CancellationToken.None);
        }

        public bool IsConnected()
        {
            return SockIO.GetState() == WebSocketState.Open;
        }

        public bool IsPaired()
        {
            return Paired;
        }

        #region Utils

        private void TimeoutOpenTasksCheck()
        {
            while(SockIO.GetState() == WebSocketState.Open)
            {
                var now = DateTime.Now;
                int count = 0;
                List<string> toRemoveKeys = new List<string>();

                foreach (var key in OpenTaskTimes.Keys.ToList())
                {
                    if ((now - OpenTaskTimes[key]).TotalMilliseconds >= TimeoutMS)
                    {
                        toRemoveKeys.Add(key);
                    }

                    //sleep checking each 10 requests
                    if((count % 10) == 0)
                    {
                        count = 0;
                        Thread.Sleep(1000);                        
                    }

                    count++;
                }

                foreach(var key in toRemoveKeys)
                {
                    TaskCompletionSource<JToken> openTask = OpenTasks[key];

                    OpenTasks.Remove(key);
                    OpenTaskTimes.Remove(key);

                    openTask.SetResult(JToken.FromObject(new ApiError()
                    {
                        code = "0",
                        isError = "true",
                        message = "Request timeout."
                    }));
                }
            }
        }

        private void HandleApiResponse(JToken data)
        {
            if (data == null && data.Children().Count() != 2)
                return;

            var idToken = data.SelectToken("id");

            if (idToken == null)
                throw new Exception("response id not found.");

            string id = idToken.ToObject<string>();

            TaskCompletionSource<JToken> openTask;
            if (!OpenTasks.TryGetValue(id, out openTask))
                return;

            OpenTasks.Remove(id);
            OpenTaskTimes.Remove(id);

            openTask.SetResult(data.SelectToken("result"));
        }

        private void HandleRekeyResponse()
        {
            GenerateNewAppKey();
            SockIO.EmitAsync("rekeyed", new
            {
                plugin = AppName,
                data = new
                {
                    origin = AppName,
                    appkey = StorageProvider.GetAppkey()
                }
            });
        }

        private void HandlePairedResponse(bool? paired)
        {
            Paired = paired.GetValueOrDefault();

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
            StorageProvider.SetAppkey("appkey:" + UtilsHelper.RandomNumber(24));
        }

        private static void ThrowOnApiError(JToken result)
        {
            if (result.Type != JTokenType.Object ||
               result.SelectToken("isError") == null)
                return;

            var apiError = result.ToObject<ApiError>();

            if (apiError != null)
                throw new Exception(apiError.message);
        }

        #endregion
    }
}
