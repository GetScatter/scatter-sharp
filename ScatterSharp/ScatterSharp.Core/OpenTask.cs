using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScatterSharp.Core
{
    public class OpenTask
    {
        public TaskCompletionSource<object> PromiseTask { get; set; }
        public DateTime TaskRequestTime { get; set; }
        public int TaskTimeoutMS { get; set; }
    }
}
