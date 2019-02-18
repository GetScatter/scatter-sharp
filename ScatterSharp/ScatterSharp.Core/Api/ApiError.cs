using System;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class ApiError
    {
        public string type;
        public string message;
        public string code;
        public string isError;
    }
}
