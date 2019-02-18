using Newtonsoft.Json.Linq;
using ScatterSharp.Core;
using ScatterSharp.Core.Api;
using ScatterSharp.Core.Interfaces;
using SocketIOSharp;
using SocketIOSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScatterSharp
{
    public class SocketService : SocketServiceBase
    {
        public SocketService(IAppStorageProvider storageProvider, SocketIOConfigurator config, string appName, int timeout = 60000) :
            base(storageProvider, config, appName, timeout)
        {
            SockIO = new SocketIO(config);
        }

        #region Utils

        protected override void StartTimeoutOpenTasksCheck()
        {
            TimoutTasksTask = Task.Run(() => TimeoutOpenTasksCheck());
        }

        protected override object WaitForOpenTasksCheck(int openTaskCheckIntervalSecs)
        {
            Thread.Sleep(openTaskCheckIntervalSecs * 1000);
            return null;
        }

        protected override object BuildApiError()
        {
            return JToken.FromObject(new ApiError()
            {
                code = "0",
                isError = "true",
                message = "Request timeout."
            });
        }

        protected override TReturn BuildApiResponse<TReturn>(object jtoken)
        {
            var result = jtoken as JToken;

            if (result.Type == JTokenType.Object &&
               result.SelectToken("isError") != null)
            {
                var apiError = result.ToObject<ApiError>();
                if (apiError != null)
                    throw new Exception(apiError.message);
            }

            return result.ToObject<TReturn>();
        }
        
        protected override void HandlePairedResponse(IEnumerable<object> args)
        {
            HandlePairedResponse(args.Cast<JToken>().First().ToObject<bool?>());
        }

        protected override void HandleApiResponse(IEnumerable<object> args)
        {
            var data = args.Cast<JToken>().First();

            if (data == null && data.Children().Count() != 2)
                return;

            var idToken = data.SelectToken("id");

            if (idToken == null)
                throw new Exception("response id not found.");

            string id = idToken.ToObject<string>();

            OpenTask openTask;
            if (!OpenTasks.TryGetValue(id, out openTask))
                return;

            OpenTasks.Remove(id);

            openTask.PromiseTask.SetResult(data.SelectToken("result"));
        }

        protected override void HandleEventResponse(IEnumerable<object> args)
        {
            var data = args.Cast<JToken>().First();

            var eventToken = data.SelectToken("event");

            if (eventToken == null)
                throw new Exception("event type not found.");

            string type = eventToken.ToObject<string>();

            List<Action<object>> eventListeners = null;

            if (EventListenersDict.TryGetValue(type, out eventListeners))
            {
                foreach (var listener in eventListeners)
                {
                    listener(data.SelectToken("payload"));
                }
            }
        }

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
