using System;
using IdentityApi.DataModels;
using IdentityApi.RequestModels;
using IdentityApi.ResponseModels;
using IdentityApi.Services.SQLServer;
using IdentityApi.Utilities;
using System.Linq;
using IdentityApi.Account;
using IdentityApi.Identifiers;
using UAParser;
using System.Net;

namespace IdentityApi.Auth
{
    public class AuthManager : IAuthManager
    {
        private readonly IdentityStore _store;
        private readonly TMCache _cache;

        public AuthManager(IdentityStore store, TMCache cache)
        {
            _store = store;
            _cache = cache;
        }

        public LogInResponse Authenticate(LogInRequest logInRequest)
        {
            LogInResponse logInResponse = new();

            string profileID = _cache.Get<String>(logInRequest.USERNAME.Trim().ToLower());
            Profile userProfile = null;

            if (!string.IsNullOrWhiteSpace(profileID))
                userProfile = _cache.Get<Profile>(Profile.PROFILE_CACHE_KEY + profileID);
            else
            {
                var userData = (from credential in _store.CREDENTIALS
                                join profile in _store.PROFILE on credential.PROFILE_ID equals profile.ID
                                where credential.USERNAME == logInRequest.USERNAME.Trim().ToLower()
                                select new
                                {
                                    dataCredential = credential,
                                    dataProfile = profile
                                }).FirstOrDefault();


                if (userData != null)
                {
                    userProfile = userData.dataProfile;
                    userProfile.CREDENTIAL = userData.dataCredential;
                }
            }
            if (userProfile != null)
            {
                UserSecret userSecret = Utility.GetUserSecret(userProfile.CREDENTIAL.SALT, logInRequest.PASSWORD);

                if (userSecret.SECRET_HASH.Equals(userProfile.CREDENTIAL.SECRET_HASH))
                {
                    logInResponse.IS_VERIFIED = userProfile.CREDENTIAL.USERNAME.Equals("system") || userProfile.EMAIL_VERIFIED;

                    if (userProfile.LOCKED)
                    {
                        logInResponse.IS_AUTHENTICATED = false;
                        logInResponse.ERRORS.Add("Profile locked due to many invalid attempts to login. Contact administartor for assistance");
                    }

                    if (userProfile.STATUS == Status.DEACTIVE)
                    {
                        logInResponse.IS_AUTHENTICATED = false;
                        logInResponse.ERRORS.Add("Your profile is temporarily deactivated. To activate your profile conatct administrator.");
                    }

                    logInResponse.IS_AUTHENTICATED = true;
                    logInResponse.PROFILE_ID = userProfile.ID;
                    logInResponse.NAME = userProfile.NAME;

                    _cache.Add<Profile>(Profile.PROFILE_CACHE_KEY + userProfile.ID, userProfile);
                    _cache.Add<String>(userProfile.CREDENTIAL.USERNAME, userProfile.ID);
                }
                else
                {
                    logInResponse.IS_AUTHENTICATED = false;

                    if (!userProfile.CREDENTIAL.USERNAME.Equals("system"))
                    {
                        if (!userProfile.LOCKED)
                        {
                            if (userProfile.LOGIN_ATTEMPTS == Profile.MAX_ALLOWED_LOGON_ATTEMPTS)
                                userProfile.LOCKED = true;
                            else
                                userProfile.LOGIN_ATTEMPTS++;

                            _store.PROFILE.Update(userProfile);
                            _store.SaveChanges();
                        }
                        else
                            logInResponse.ERRORS.Add("Invalid password");
                    }
                }
            }
            else
            {
                logInResponse.IS_AUTHENTICATED = false;
                logInResponse.ERRORS.Add("User not found");
            }

            return logInResponse;
        }

        public string GenerateJwtToken(LogInResponse logInResponse)
        {
            throw new NotImplementedException();
        }

        public RefreshToken GenerateRefreshToken(string profileID, UserAgent ua, IPAddress ipAddress)
        {
            throw new NotImplementedException();
        }

        public RefreshToken GetOrCreateRefreshToken(string profileID, UserAgent ua, IPAddress ipAddress)
        {
            throw new NotImplementedException();
        }

        public RefreshToken GetRefreshToken(string profileID, UserAgent ua, IPAddress ipAddress)
        {
            throw new NotImplementedException();
        }

        public void Logout(string profileID, UserAgent ua, IPAddress ipAddress)
        {
            throw new NotImplementedException();
        }

        public void UpdateRefreshToken(RefreshToken token)
        {
            throw new NotImplementedException();
        }
    }
}
