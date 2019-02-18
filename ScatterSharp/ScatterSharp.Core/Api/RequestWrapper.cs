using System;
using System.Collections.Generic;
using System.Text;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class RequestWrapper
    {
        public object data;
        public string plugin;
    }
}
