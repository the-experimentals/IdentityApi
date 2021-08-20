using System;
using System.Collections.Generic;

namespace IdentityApi.ResponseModels
{
    public class ChangeRequestResponse
    {
        public bool IS_CHANGED { get; set; }
        public List<string> ERRORS { get; set; } = new();
    }
}
