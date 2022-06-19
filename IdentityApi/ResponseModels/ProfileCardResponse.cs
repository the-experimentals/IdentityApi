namespace IdentityApi.ResponseModels;

public class ProfileCardResponse
{
    public string PROFILE_ID { get; set; }
    public string USERNAME { get; set; }
    public string NAME { get; set; }
    public bool LOCKED { get; set; }
    public string INITIALS { get; set; }
}
