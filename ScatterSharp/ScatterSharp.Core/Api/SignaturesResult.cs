using System;
using System.Collections.Generic;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class SignaturesResult
    {
        public List<string> signatures;
        public IdentityRequiredFields returnedFields;
    }
}
