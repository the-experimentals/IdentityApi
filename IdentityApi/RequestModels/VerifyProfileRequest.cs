using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityApi.RequestModels
{
    public class VerifyProfileRequest
    { 
        [Required]
        public string OTP { get; set; }
    }
}
