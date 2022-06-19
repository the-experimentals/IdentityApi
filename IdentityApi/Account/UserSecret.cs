namespace IdentityApi.Account;

public class UserSecret
{
    public byte[] SALT { get; set; }
    public string SECRET_HASH { get; set; }
}
