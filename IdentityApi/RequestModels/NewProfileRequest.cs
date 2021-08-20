using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityApi.RequestModels
{
    public class NewProfileRequest
    {
        [Required(ErrorMessage = "Username for your new profile is missing")]
        [MinLength(5, ErrorMessage = "Username for your new profile cannot be less than 5 characters")]
        public string USERNAME { get; set; }

        [Required(ErrorMessage = "Password for your new profile is missing")]
        [MinLength(5, ErrorMessage = "Password for your new profile cannot be less than 8 characters")]
        public string PASSWORD { get; set; }

        [Required(ErrorMessage = "Confirm password for your new profile is missing")]
        [MinLength(5, ErrorMessage = "Confirm password for your new profile cannot be less than 8 characters")]
        [Compare("SECRET", ErrorMessage = "Confirm password didnt matched with password")]
        public string CONFIRM_PASSWORD { get; set; }

        [Required(ErrorMessage = "First name for your new profile is missing")]
        public string FIRST_NAME { get; set; }

        [Required(ErrorMessage = "Last name for your new profile is missing")]
        public string LAST_NAME { get; set; }

        [Required(ErrorMessage = "E-mail for your profile is missing")]
        [DataType(DataType.EmailAddress)]
        public string EMAIL { get; set; }

        public bool IS_SAVED { get; set; } = false;

        public List<string> ERRORS { get; set; } = new List<string>();
    }
}
