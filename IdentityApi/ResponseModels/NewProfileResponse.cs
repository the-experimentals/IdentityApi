using System.Collections.Generic;

namespace IdentityApi.ResponseModels;

public class NewProfileResponse
{
    public bool IS_SAVED { get; set; }
    public List<string> list { get; set; } = new();
}
