using ScatterSharp.Core.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScatterSharp.Core.Interfaces
{
    public interface ISocketService : IDisposable
    {
        /// <summary>
        /// Link to scatter application by connecting, registering events and pair with passthrough
        /// </summary>
        /// <param name="uri">Uri to link to</param>
        /// <returns></returns>
        Task<bool> Link(Uri uri);

        /// <summary>
        /// Pair appication to registered applications in scatter
        /// </summary>
        /// <param name="passthrough">pass through rekey process</param>
        /// <returns></returns>
        Task Pair(bool passthrough = false);

        /// <summary>
        /// Send api request to scatter
        /// </summary>
        /// <typeparam name="TRequest">Request type param</typeparam>
        /// <typeparam name="TReturn">Return type param</typeparam>
        /// <param name="request">Request object</param>
        /// <param name="timeout">set response timeout that overrides the default one</param>
        /// <returns></returns>
        Task<TReturn> SendApiRequest<TRequest, TReturn>(Request<TRequest> request, int? timeout = null);

        /// <summary>
        /// Disconnect from socket
        /// </summary>
        /// <returns></returns>
        Task Disconnect();

        /// <summary>
        /// Check if socket connection is open
        /// </summary>
        /// <returns></returns>
        bool IsConnected();

        /// <summary>
        /// Check if socket service is paired with scatter
        /// </summary>
        /// <returns></returns>
        bool IsPaired();

        /// <summary>
        /// Register listener for socketio event type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        void On(string type, Action<object> callback);

        /// <summary>
        /// Remove listener by event type
        /// </summary>
        /// <param name="type">event type</param>
        void Off(string type);

        /// <summary>
        /// Remove listener by event type and position
        /// </summary>
        /// <param name="type">event type</param>
        /// <param name="index">position</param>
        void Off(string type, int index);

        /// <summary>
        /// Remove listener by callback instance
        /// </summary>
        /// <param name="callback"></param>
        void Off(Action<object> callback);

        /// <summary>
        /// remove listner by event type and callback instance
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        void Off(string type, Action<object> callback);
    }
}
