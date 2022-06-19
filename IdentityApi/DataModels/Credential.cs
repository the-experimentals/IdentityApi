using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityApi.DataModels;

public class Credential
{
    [Key] public string ID { get; set; }

    [Required] public string PROFILE_ID { get; set; }

    [Required]
    [MaxLength(15)]
    [MinLength(6)]
    public string USERNAME { get; set; }

    [Required] public string SECRET_HASH { get; set; }

    [NotMapped] public string PASSWORD { get; set; }

    [Required] public byte[] SALT { get; set; }

    [ForeignKey("PROFILE_ID")] public Profile PROFILE { get; set; }
}
