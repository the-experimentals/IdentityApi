using System;
using System.ComponentModel.DataAnnotations;
using IdentityApi.Identifiers;

namespace IdentityApi.DataModels;

public class RefreshToken
{
    public const string REFRESH_TOKEN_CACHE_KEY = "CACHE_REFRESH_TOKEN_";

    [Key] public string ID { get; set; }

    [Required] public string TOKEN { get; set; }

    [Required] public DateTime GENERATED_ON { get; set; }

    [Required] public string PROFILE_ID { get; set; }

    [Required] public int LIFE_SPAN { get; set; }

    [Required] public string DEVICE { get; set; }

    [Required] public string BROWSER { get; set; }

    [Required] public string OS { get; set; }

    [Required] public string IPv4 { get; set; }

    [Required] public string SHA { get; set; }

    [Required] public bool ACTIVE { get; set; }

    [Required] public DateTime REFRESHED_ON { get; set; }

    [Required] public Status STATUS { get; set; }
}
