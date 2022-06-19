using System.Net;
using IdentityApi.DataModels;
using IdentityApi.RequestModels;
using IdentityApi.ResponseModels;

namespace IdentityApi.Auth;

public interface IAuthManager
{
    public LogInResponse Authenticate(LogInRequest logInRequest);
    public RefreshToken GetOrCreateRefreshToken(string profileID, UserAgent ua, IPAddress ipAddress);
    public RefreshToken GenerateRefreshToken(string profileID, UserAgent ua, IPAddress ipAddress);
    public RefreshToken GetRefreshToken(string profileID, UserAgent ua, IPAddress ipAddress);
    public bool UpdateRefreshToken(RefreshToken token);
    public string GenerateJwtToken(LogInResponse logInResponse);
    public bool Logout(string profileID, UserAgent ua, IPAddress ipAddress);
    public bool DeleteRefreshToken(RefreshToken refreshToken);
}
