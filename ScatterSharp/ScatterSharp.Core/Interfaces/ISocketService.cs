using ScatterSharp.Core.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScatterSharp.Core.Interfaces
{
    public interface ISocketService : IDisposable
    {
        Task<bool> Link(Uri uri);
        Task Pair(bool passthrough = false);
        Task<TReturn> SendApiRequest<TReturn>(Request request, int? timeout = null);
        Task Disconnect();
        bool IsConnected();
        bool IsPaired();
        void On(string type, Action<object> callback);
        void Off(string type);
        void Off(string type, int index);
        void Off(Action<object> callback);
        void Off(string type, Action<object> callback);
    }
}
