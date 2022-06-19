using System.ComponentModel.DataAnnotations;

namespace IdentityApi.RequestModels;

public class LogInRequest
{
    [Required(ErrorMessage = "Your username is missing")]
    [MinLength(5, ErrorMessage = "Your username cannot be lesser than 5 characters")]
    public string USERNAME { get; set; }

    [Required(ErrorMessage = "Your password is missing")]
    [MinLength(5, ErrorMessage = "Your password cannot be lesser than 8 characters")]
    public string PASSWORD { get; set; }

    public bool REMEMBER_ME { get; set; }
}
