using System;
using System.Collections.Generic;

namespace ScatterSharp.Core.Api
{
    [Serializable]
    public class IdentityRequiredFields
    {
        public List<Network> accounts;
        public List<PersonalFields> personal;
        public List<LocationFields> location;
    }
}
