using System;
using System.Threading.Tasks;

namespace ScatterSharp.Core
{
    /// <summary>
    /// Represents all the information to open task request to scatter
    /// </summary>
    public class OpenTask
    {
        /// <summary>
        /// Promise task to obtain response from
        /// </summary>
        public TaskCompletionSource<object> PromiseTask { get; set; }

        /// <summary>
        /// Request date time
        /// </summary>
        public DateTime TaskRequestTime { get; set; }

        /// <summary>
        /// Timeout in milliseconds for the task
        /// </summary>
        public int TaskTimeoutMS { get; set; }
    }
}
