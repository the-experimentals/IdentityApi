using System;
using System.Collections.Generic;

namespace IdentityApi.ResponseModels
{
    public class LogInResponse
    {
        public bool IS_AUTHENTICATED { get; set; }
        public bool IS_VERIFIED { get; set; }
        public List<string> ERRORS { get; set; } = new List<string>();
        public string PROFILE_ID { get; set; }
        public string NAME { get; set; }
        public bool HAS_PWNED_PASSWORD { get; set; }

        public TokenResponse TOKEN { get; set; } = new();
    }
}
