namespace IdentityApi.Data;

public class JwtSecretKey
{
    public string SECRET { get; set; }
    public string ISSUER { get; set; }
    public string AUDIENCE { get; set; }
    public int TTL { get; set; }
}
