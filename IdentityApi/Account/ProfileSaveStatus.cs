using System.Collections.Generic;

namespace IdentityApi.Account;

public class ProfileSaveStatus
{
    public bool IS_SAVED { get; set; }
    public List<string> ERRORS { get; set; } = new();
    public string PROFILE_ID { get; set; }
}
