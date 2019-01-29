using System;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    class UpdateIdentityRequest : ApiBase
    {
        public string name;
        public string kyc;
    }
}