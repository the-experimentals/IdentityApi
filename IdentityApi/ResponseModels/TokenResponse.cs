using System;
namespace IdentityApi.ResponseModels
{
    public class TokenResponse
    {
        public string ACCESS { get; set; }
        public string REFRESH { get; set; }
        public bool ALLOW_REFRESH { get; set; }
        public bool IS_REFRESHED { get; set; }
        public int TTL { get; set; } = 5;
    }
}
