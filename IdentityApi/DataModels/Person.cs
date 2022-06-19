using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityApi.DataModels;

public class Person
{
    [Key] public string ID { get; set; }

    [Required] public string PROFILE_ID { get; set; }

    [Required] public string FIRST_NAME { get; set; }

    [Required] public string LAST_NAME { get; set; }

    [ForeignKey("PROFILE_ID")] public Profile PROFILE { get; set; }

    //[Required]
    //public string PHONE_NUMBER { get; set; }
}
