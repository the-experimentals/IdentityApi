using System.Collections.Generic;

namespace IdentityApi.ResponseModels;

public class VerificationCodeResponse
{
    public bool SENT { get; set; }
    public List<string> ERRORS { get; set; } = new();
}
