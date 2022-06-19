using System.ComponentModel.DataAnnotations;

namespace IdentityApi.RequestModels;

public class RefreshTokenRequest
{
    [Required] public string REFRESH { get; set; }
}
