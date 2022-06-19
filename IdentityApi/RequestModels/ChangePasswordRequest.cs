using System.ComponentModel.DataAnnotations;

namespace IdentityApi.RequestModels;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Your old password cannot be empty")]
    [MinLength(8, ErrorMessage = "Your old password cannot be lesser than 8 characters")]
    public string OLD_PASSWORD { get; set; }

    [Required(ErrorMessage = "Your new password cannot be empty")]
    [MinLength(8, ErrorMessage = "Your new password cannot be lesser than 8 characters")]
    public string NEW_PASSWORD { get; set; }

    [Required(ErrorMessage = "Your confirm password cannot be empty")]
    [MinLength(8, ErrorMessage = "Your confirm password cannot be lesser than 8 characters")]
    [Compare("NEW_PASSWORD", ErrorMessage = "your new password and confirm password didnt matched")]
    public string CONFIRM_PASSWORD { get; set; }
}
