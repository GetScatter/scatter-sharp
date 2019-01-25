using Cryptography.ECDSA;
using ScatterSharp.Core.Api;
using ScatterSharp.Core.Helpers;
using ScatterSharp.Core.Interfaces;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using System.Threading;
using SocketIOSharp.Core;

namespace ScatterSharp.Core
{
    public abstract class SocketServiceBase : ISocketService 
    {
        protected readonly int OPEN_TASK_CHECK_INTERVAL_SECS = 1;
        protected bool Paired { get; set; }
        protected IAppStorageProvider StorageProvider { get; set; }
        protected string AppName { get; set; }
        protected int TimeoutMS { get; set; }

        protected ISocketIO SockIO { get; set; }
        protected Dictionary<string, OpenTask> OpenTasks { get; set; }
        protected TaskCompletionSource<bool> PairOpenTask { get; set; }
        protected Task TimoutTasksTask { get; set; }

        public SocketServiceBase(IAppStorageProvider storageProvider, SocketIOConfigurator config, string appName, int timeout = 60000)
        {
            OpenTasks = new Dictionary<string, OpenTask>();

            StorageProvider = storageProvider;
            AppName = appName;
            TimeoutMS = timeout;
        }

        public void Dispose()
        {
            SockIO.Dispose();
            StorageProvider.Save();
        }

        public bool IsPaired()
        {
            return Paired;
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

            SockIO.On("paired", HandlePairedResponse);
            SockIO.On("rekey", HandleRekeyResponse);
            SockIO.On("api", HandleApiResponse);

            StartTimeoutOpenTasksCheck();

            await Pair(true);
            return true;
        }

        public async Task<TReturn> SendApiRequest<TReturn>(Request request, int? timeout = null)
        {
            if (request.type == "identityFromPermissions" && !Paired)
            {
                return default(TReturn);
            }

            await Pair();

            if (!Paired)
                throw new Exception("The user did not allow this app to connect to their Scatter");

            var tcs = new TaskCompletionSource<object>();

            request.id = UtilsHelper.RandomNumber(24);
            request.appkey = StorageProvider.GetAppkey();
            request.nonce = StorageProvider.GetNonce() ?? "";

            var nextNonce = UtilsHelper.RandomNumberBytes();
            request.nextNonce = UtilsHelper.ByteArrayToHexString(Sha256Manager.GetHash(nextNonce));
            StorageProvider.SetNonce(UtilsHelper.ByteArrayToHexString(nextNonce));

            OpenTasks.Add(request.id, new OpenTask()
            {
                PromiseTask = tcs,
                TaskRequestTime = DateTime.Now,
                TaskTimeoutMS = timeout.HasValue ? timeout.Value : TimeoutMS
            });

            await SockIO.EmitAsync("api", new { data = request, plugin = AppName });

            return BuildApiResponse<TReturn>(await tcs.Task);
        }

        public Task Disconnect()
        {
            return SockIO.DisconnectAsync();
        }

        public bool IsConnected()
        {
            return SockIO.GetState() == WebSocketState.Open;
        }

        #region Utils

        protected IEnumerator TimeoutOpenTasksCheck()
        {
            while (SockIO.GetState() == WebSocketState.Open)
            {
                var now = DateTime.Now;
                int count = 0;
                List<string> toRemoveKeys = new List<string>();

                foreach (var key in OpenTasks.Keys.ToList())
                {
                    OpenTask openTask;
                    if (!OpenTasks.TryGetValue(key, out openTask))
                    {
                        continue;
                    }

                    if ((now - openTask.TaskRequestTime).TotalMilliseconds >= openTask.TaskTimeoutMS)
                    {
                        toRemoveKeys.Add(key);
                    }

                    //sleep checking each 10 requests
                    if ((count % 10) == 0)
                    {
                        count = 0;
                        Thread.Sleep(OPEN_TASK_CHECK_INTERVAL_SECS * 1000);
                        yield return null;
                    }

                    count++;
                }

                foreach (var key in toRemoveKeys)
                {
                    OpenTask openTask;
                    if (!OpenTasks.TryGetValue(key, out openTask))
                        continue;

                    OpenTasks.Remove(key);

                    openTask.PromiseTask.SetResult(BuildApiError());
                }

                Thread.Sleep(OPEN_TASK_CHECK_INTERVAL_SECS * 1000);
                yield return null;
            }
        }

        protected void HandlePairedResponse(bool? paired)
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

            if (PairOpenTask != null)
            {
                PairOpenTask.SetResult(Paired);
            }
        }

        protected void GenerateNewAppKey()
        {
            StorageProvider.SetAppkey("appkey:" + UtilsHelper.RandomNumber(24));
        }

        protected abstract TReturn BuildApiResponse<TReturn>(object jtoken);

        protected abstract void StartTimeoutOpenTasksCheck();

        protected abstract object BuildApiError();

        protected abstract void HandlePairedResponse(IEnumerable<object> args);

        protected abstract void HandleApiResponse(IEnumerable<object> args);

        private void HandleRekeyResponse(IEnumerable<object> args)
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

        #endregion
    }
}
