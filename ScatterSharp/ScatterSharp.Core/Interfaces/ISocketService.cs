using ScatterSharp.Core.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScatterSharp.Core.Interfaces
{
    public interface ISocketService : IDisposable
    {
        Task Link(Uri uri);
        Task Pair(bool passthrough = false);
        Task<TReturn> SendApiRequest<TReturn>(Request request);
        Task Disconnect();
        bool IsConnected();
        bool IsPaired();
    }
}
