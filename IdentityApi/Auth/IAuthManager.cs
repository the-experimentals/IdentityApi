using System;
using IdentityApi.RequestModels;
using IdentityApi.ResponseModels;

namespace IdentityApi.Auth
{
    public interface IAuthManager
    {
        public LogInResponse Authenticate(LogInRequest logInRequest);
    }
}
