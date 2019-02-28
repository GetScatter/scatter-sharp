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
    /// <summary>
    /// Base implementation for socket service using socketio-sharp interface
    /// </summary>
    public abstract class SocketServiceBase : ISocketService 
    {
        protected readonly int OPEN_TASK_CHECK_INTERVAL_SECS = 1;
        protected bool Paired { get; set; }
        protected IAppStorageProvider StorageProvider { get; set; }
        protected string AppName { get; set; }
        protected int TimeoutMS { get; set; }

        protected ISocketIO SockIO { get; set; }
        protected Dictionary<string, OpenTask> OpenTasks { get; set; }
        protected string PairOpenId { get; set; }
        protected Task TimoutTasksTask { get; set; }
        protected Dictionary<string, List<Action<object>>> EventListenersDict { get; set; }

        /// <summary>
        /// Constructor for socket service
        /// </summary>
        /// <param name="storageProvider">Storage service for appName and nonce</param>
        /// <param name="config">socketio-sharp configurator</param>
        /// <param name="appName">app name</param>
        /// <param name="timeout">default app request timeout in milliseconds</param>
        public SocketServiceBase(IAppStorageProvider storageProvider, SocketIOConfigurator config, string appName, int timeout = 60000)
        {
            OpenTasks = new Dictionary<string, OpenTask>();
            EventListenersDict = new Dictionary<string, List<Action<object>>>();

            if (storageProvider == null)
                throw new ArgumentNullException("storageProvider");

            StorageProvider = storageProvider;
            AppName = appName;
            TimeoutMS = timeout;

            PairOpenId = UtilsHelper.RandomNumber(24);
        }

        /// <summary>
        /// dispose socketio-sharp websocket
        /// </summary>
        public void Dispose()
        {
            SockIO.Dispose();
            StorageProvider.Save();
        }

        /// <summary>
        /// Link to scatter application by connecting, registering events and pair with passthrough
        /// </summary>
        /// <param name="uri">Uri to link to</param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<bool> Link(Uri uri, int? timeout = null)
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
            SockIO.On("event", HandleEventResponse);

            StartTimeoutOpenTasksCheck();

            await Pair(true, timeout);
            return true;
        }

        /// <summary>
        /// Pair appication to registered applications in scatter
        /// </summary>
        /// <param name="passthrough">pass through rekey process</param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task Pair(bool passthrough = false, int? timeout = null)
        {
            var pairOpenTask = new TaskCompletionSource<object>();
            var openTask = new OpenTask()
            {
                PromiseTask = pairOpenTask,
                TaskRequestTime = DateTime.Now,
                TaskTimeoutMS = timeout.HasValue ? timeout.Value : TimeoutMS
            };

            if (OpenTasks.ContainsKey(PairOpenId))
                OpenTasks[PairOpenId] = openTask;
            else
                OpenTasks.Add(PairOpenId, openTask);

            await SockIO.EmitAsync("pair", new RequestWrapper()
            {
                data = new PairRequest()
                {
                    appkey = StorageProvider.GetAppkey(),
                    passthrough = passthrough,
                    origin = AppName
                },
                plugin = AppName
            });

            await pairOpenTask.Task;
        }

        /// <summary>
        /// Send api request to scatter
        /// </summary>
        /// <typeparam name="TRequest">Request type param</typeparam>
        /// <typeparam name="TReturn">Return type param</typeparam>
        /// <param name="request">Request object</param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        public async Task<TReturn> SendApiRequest<TRequest, TReturn>(Request<TRequest> request, int? timeout = null)
        {
            if (request.type == "identityFromPermissions" && !Paired)
            {
                return default(TReturn);
            }

            await Pair();

            if (!Paired)
                throw new Exception("The user did not allow this app to connect to their Scatter");

            var tcs = new TaskCompletionSource<object>();

            do
            {
                request.id = UtilsHelper.RandomNumber(24);
            }
            while (OpenTasks.ContainsKey(request.id));
            
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

            await SockIO.EmitAsync("api", new RequestWrapper()
            {
                data = request,
                plugin = AppName
            });

            return BuildApiResponse<TReturn>(await tcs.Task);
        }

        /// <summary>
        /// Disconnect from socket
        /// </summary>
        /// <returns></returns>
        public Task Disconnect()
        {
            return SockIO.DisconnectAsync();
        }

        /// <summary>
        /// Check if socket connection is open
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return SockIO.GetState() == WebSocketState.Open;
        }

        /// <summary>
        /// Check if socket service is paired with scatter
        /// </summary>
        /// <returns></returns>
        public bool IsPaired()
        {
            return Paired;
        }

        /// <summary>
        /// Register listener for socketio event type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public void On(string type, Action<object> callback)
        {
            if (callback == null)
                return;

            List<Action<object>> eventListeners = null;

            if (EventListenersDict.TryGetValue(type, out eventListeners))
            {
                eventListeners.Add(callback);
            }
            else
            {
                EventListenersDict.Add(type, new List<Action<object>>() { callback });
            }
        }

        /// <summary>
        /// Remove listener by event type
        /// </summary>
        /// <param name="type">event type</param>
        public void Off(string type)
        {
            if (EventListenersDict.ContainsKey(type))
                EventListenersDict[type].Clear();
        }

        /// <summary>
        /// Remove listener by event type and position
        /// </summary>
        /// <param name="type">event type</param>
        /// <param name="index">position</param>
        public void Off(string type, int index)
        {
            if (EventListenersDict.ContainsKey(type))
                EventListenersDict[type].RemoveAt(index);
        }

        /// <summary>
        /// Remove listener by callback instance
        /// </summary>
        /// <param name="callback"></param>
        public void Off(Action<object> callback)
        {
            foreach (var el in EventListenersDict.Values)
            {
                el.Remove(callback);
            }
        }

        /// <summary>
        /// remove listner by event type and callback instance
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public void Off(string type, Action<object> callback)
        {
            if (EventListenersDict.ContainsKey(type))
                EventListenersDict[type].Remove(callback);
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
                    if ((count % ScatterConstants.OPEN_TASK_NR_CHECK) == 0)
                    {
                        count = 0;
                        yield return WaitForOpenTasksCheck(ScatterConstants.OPEN_TASK_CHECK_INTERVAL_SECS);
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
                yield return WaitForOpenTasksCheck(ScatterConstants.OPEN_TASK_CHECK_INTERVAL_SECS);
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

            OpenTask openTask;
            if (!OpenTasks.TryGetValue(PairOpenId, out openTask))
                return;

            openTask.PromiseTask.SetResult(Paired);
            openTask.TaskTimeoutMS = int.MaxValue;
        }

        protected void GenerateNewAppKey()
        {
            StorageProvider.SetAppkey("appkey:" + UtilsHelper.RandomNumber(24));
        }

        protected abstract TReturn BuildApiResponse<TReturn>(object jtoken);

        protected abstract void StartTimeoutOpenTasksCheck();

        protected abstract object WaitForOpenTasksCheck(int openTaskCheckIntervalSecs);

        protected abstract object BuildApiError();

        protected abstract void HandlePairedResponse(IEnumerable<object> args);

        protected abstract void HandleApiResponse(IEnumerable<object> args);

        protected abstract void HandleEventResponse(IEnumerable<object> args);

        private void HandleRekeyResponse(IEnumerable<object> args)
        {
            GenerateNewAppKey();
            SockIO.EmitAsync("rekeyed", new RequestWrapper()
            {
                plugin = AppName,
                data = new RekeyRequest()
                {
                    origin = AppName,
                    appkey = StorageProvider.GetAppkey()
                }
            });
        }

        #endregion
    }
}
