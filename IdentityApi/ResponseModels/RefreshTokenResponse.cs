using System;
using System.Collections.Generic;

namespace IdentityApi.ResponseModels
{
    public class RefreshTokenResponse : TokenResponse
    {
        public new bool IS_REFRESHED { get; set; }
        public List<string> ERRORS { get; set; } = new List<string>();
    }
}
