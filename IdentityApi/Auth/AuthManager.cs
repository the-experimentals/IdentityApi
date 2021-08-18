using System;
using IdentityApi.DataModels;
using IdentityApi.RequestModels;
using IdentityApi.ResponseModels;
using IdentityApi.Services.SQLServer;
using IdentityApi.Utilities;
using System.Linq;
using IdentityApi.Account;
using IdentityApi.Identifiers;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using IdentityApi.Data;

namespace IdentityApi.Auth
{
    public class AuthManager : IAuthManager
    {
        private readonly IdentityStore _store;
        private readonly TMCache _cache;
        private readonly JwtSecretKey _jwtSecretKey;

        public AuthManager(IdentityStore store, TMCache cache, IOptions<JwtSecretKey> jwtSecretKey)
        {
            _store = store;
            _cache = cache;
            _jwtSecretKey = jwtSecretKey.Value;
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
                UserSecret userSecret = Utilities.Utility.GetUserSecret(userProfile.CREDENTIAL.SALT, logInRequest.PASSWORD);

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
            JwtSecurityTokenHandler tokenHandler = new();

            List<Claim> claims = new();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, logInResponse.PROFILE_ID));
            claims.Add(new Claim(ClaimTypes.Name, logInResponse.NAME));

            if (!logInResponse.IS_VERIFIED)
            {
                claims.Add(new Claim(ClaimTypes.Role, "TEMPORARY"));
            }

            ClaimsIdentity claimsIdentity = new(claims);

            byte[] key = Encoding.ASCII.GetBytes(_jwtSecretKey.SECRET);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Issuer = _jwtSecretKey.ISSUER,
                Audience = _jwtSecretKey.ISSUER,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSecretKey.TTL),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(string profileID, UserAgent ua, IPAddress ipAddress)
        {
            throw new NotImplementedException();
        }

        public RefreshToken GetOrCreateRefreshToken(string profileID, UserAgent ua, IPAddress ipAddress)
        {
            RefreshToken refreshToken = GetRefreshToken(profileID, ua, ipAddress);

            if (refreshToken == null)
            {
                refreshToken = GenerateRefreshToken(profileID, ua, ipAddress);
                _cache.Add<RefreshToken>(RefreshToken.REFRESH_TOKEN_CACHE_KEY + refreshToken.SHA, refreshToken);
            }

            return refreshToken;
        }

        public RefreshToken GetRefreshToken(string profileID, UserAgent ua, IPAddress ipAddress)
        {
            if (ua == null)
                throw new InvalidOperationException();

            string sha = GetSHA(profileID, ua, ipAddress);

            RefreshToken refreshToken = _cache.Get<RefreshToken>(RefreshToken.REFRESH_TOKEN_CACHE_KEY + sha);

            if (refreshToken == null)
                //check if refresh token already exist in store for a profile
                refreshToken = _store.REFRESH_TOKENS.Where(x => x.SHA.Equals(sha) && x.STATUS != Status.DELETED).FirstOrDefault();


            if (refreshToken != null)
            {
                DateTime tokenTimeSpan = refreshToken.GENERATED_ON.AddDays(refreshToken.LIFE_SPAN);

                if (DateTime.UtcNow > tokenTimeSpan)
                {
                    // refresh token is expired.
                    DeleteRefreshToken(refreshToken);
                    refreshToken = GenerateRefreshToken(profileID, ua, ipAddress);
                }
                else
                {
                    refreshToken.ACTIVE = true;
                    refreshToken.REFRESHED_ON = DateTime.UtcNow;

                    _store.REFRESH_TOKENS.Update(refreshToken);
                    _store.SaveChanges();
                }

                _cache.Add<RefreshToken>(RefreshToken.REFRESH_TOKEN_CACHE_KEY + refreshToken.SHA, refreshToken);
            }

            return refreshToken;
        }
        
        public void DeleteRefreshToken(RefreshToken refreshToken)
        {
            refreshToken.STATUS = Status.DELETED;

            _store.REFRESH_TOKENS.Update(refreshToken);
            _store.SaveChanges();
        }

        public void Logout(string profileID, UserAgent ua, IPAddress ipAddress)
        {
            throw new NotImplementedException();
        }

        public void UpdateRefreshToken(RefreshToken token)
        {
            throw new NotImplementedException();
        }

        private string GetSHA(string profileID, RequestModels.UserAgent ua, IPAddress ipAddress)
        {
            return Utilities.Utility.ComputeSHA(string.Concat(profileID, ua.DEVICE, ua.BROWSER, ua.BROWSER, ipAddress));
        }
    }
}
