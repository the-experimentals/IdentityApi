using System;
namespace IdentityApi.ResponseModels
{
    public class ProfileCardResponse
    {
        public string USERNAME { get; set; }
        public string NAME { get; set; }
        public bool LOCKED { get; set; }
        public string INITIALS { get; set; }
    }
}
