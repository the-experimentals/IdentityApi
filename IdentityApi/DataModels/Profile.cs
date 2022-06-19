using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IdentityApi.Identifiers;

namespace IdentityApi.DataModels;

public class Profile
{
    public const string PROFILE_CACHE_KEY = "CACHE_PROFILE_";
    public const string ADMIN_GUID = "af8f3217-02b2-4cc2-b536-74f99def2a39";
    public static int MAX_ALLOWED_LOGON_ATTEMPTS = 5;

    [Key] public string ID { get; set; }

    [Required] public string NAME { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public string EMAIL { get; set; }

    [Required] public DateTime CREATED_ON { get; set; }

    [Required] public string CREATED_BY { get; set; }

    public DateTime? MODIFIED_ON { get; set; }
    public string MODIFIED_BY { get; set; }

    [Required] public bool LOCKED { get; set; }


    [Required] public int LOGIN_ATTEMPTS { get; set; }

    [Required] public string LANGUAGE { get; set; } = Languages.ENGLISH_US;

    [Required] public bool EMAIL_VERIFIED { get; set; }

    [Required] public Status STATUS { get; set; } = Status.ACTIVE;

    [NotMapped] public Credential CREDENTIAL { get; set; }

    [NotMapped] public bool NEW { get; set; }

    [NotMapped] public Person PERSON { get; set; }
    //[NotMapped]
    //public ProfileRole PROFILE_ROLE { get; set; }
    //[NotMapped]
    //public List<RolePermission> ROLE_PERMISSIONS { get; set; }
}
