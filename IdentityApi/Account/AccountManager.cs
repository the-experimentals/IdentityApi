using System;
using System.Collections.Generic;
using System.Linq;
using IdentityApi.DataModels;
using IdentityApi.RequestModels;
using IdentityApi.ResponseModels;
using IdentityApi.Services.SQLServer;
using IdentityApi.Utilities;
using Microsoft.EntityFrameworkCore.Storage;

namespace IdentityApi.Account
{
    public class AccountManager : IAccountManager
    {
        private readonly string OTP_CACHE_KEY = "CACHE_OTP_";
        private readonly IdentityStore _store;
        private readonly TMCache _cache;

        public AccountManager(IdentityStore store, TMCache cache)
        {
            _store = store;
            _cache = cache;
        }

        public ProfileSaveStatus CreateProfile(Profile profile)
        {
            ProfileSaveStatus status = new();

            Credential credential = profile.CREDENTIAL;

            if(profile.NEW)
            {
                // check credentials for new profile already exist in records.
                if (!_store.CREDENTIALS.Any(c => c.USERNAME.Equals(credential.USERNAME.ToLower().Trim())))
                {
                    Person person = profile.PERSON;

                    if (string.IsNullOrWhiteSpace(profile.ID))
                        profile.ID = Guid.NewGuid().ToString();

                    //if (person != null)
                    //    profile.NAME = person.FIRST_NAME + " " + person.LAST_NAME;

                    profile.CREATED_ON = DateTime.Now;
                }
                else
                {
                    status.IS_SAVED = false;
                    status.ERRORS.Add("Username already exist. Please use other username to register.");
                    return status;
                }
            }
            else
            {
                // update profile.
            }

            // Begin transaction for creating/updating profile in database.
            using IDbContextTransaction transaction = _store.Database.BeginTransaction();

            try
            {
                if (profile.NEW)
                    _store.PROFILE.Add(profile);
                else
                    _store.PROFILE.Update(profile);

                _store.SaveChanges();

                if (profile.NEW)
                {
                    credential.PROFILE_ID = profile.ID;
                    CreateCredentials(credential);
                }

                transaction.Commit();
                status.IS_SAVED = true;
                status.PROFILE_ID = profile.ID;
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.StackTrace);
            }

            return status;
        }

        public List<ProfileCardResponse> GetProfiles()
        {
            var profiles = (from profile in _store.PROFILE
                            join credential in _store.CREDENTIALS
                            on profile.ID equals credential.PROFILE_ID
                            where credential.USERNAME != "system"
                            select new
                            {
                                dataProfile = profile,
                                dataCredential = credential
                            }).ToList();

            List<ProfileCardResponse> profileViews = new();

            if (profiles.Any())
            {
                foreach (var profile in profiles)
                {
                    profileViews.Add(new ProfileCardResponse
                    {
                        USERNAME = profile.dataCredential.USERNAME,
                        NAME = profile.dataProfile.NAME,
                        LOCKED = profile.dataProfile.LOCKED,
                        INITIALS = GetNameInitials(profile.dataProfile.NAME)
                    });
                }
            }

            return profileViews;

        }

        private string GetNameInitials(string name)
        {
            string initials = "";

            if (name.Length == 0)
                return "";

            string[] splitName = name.Split(" ");

            foreach (string sp in splitName)
            {
                initials += sp[0];
            }

            return initials.ToUpper();
        }

        private void CreateCredentials(Credential credential)
        {
            // create new credentials
            // first check if that credential already exist for that profile

            //must send user identifire and secret

            Credential existingCredential = _store.CREDENTIALS.FirstOrDefault(c => c.PROFILE_ID.Equals(credential.PROFILE_ID));
            if (existingCredential == null)
            {
                // get credential type opted
                // for now username credential supported

                UserSecret userSecret = Utility.GetUserSecret(null, credential.PASSWORD);

                Credential newCredential = new()
                {
                    ID = Guid.NewGuid().ToString(),
                    USERNAME = credential.USERNAME.Trim().ToLower(),
                    SECRET_HASH = userSecret.SECRET_HASH,
                    PROFILE_ID = credential.PROFILE_ID,
                    SALT = userSecret.SALT,
                };

                _store.CREDENTIALS.Add(newCredential);
                _store.SaveChanges();

            }
        }

        public ChangeRequestResponse ChangePassword(string profileID, ChangePasswordRequest changePasswordRequest)
        {
            Profile profile = GetProfile(profileID);

            ChangeRequestResponse response = new();

            UserSecret newPasswordSecret = Utility.GetUserSecret(profile.CREDENTIAL.SALT, changePasswordRequest.NEW_PASSWORD.Trim());

            if (newPasswordSecret.SECRET_HASH.Equals(profile.CREDENTIAL.SECRET_HASH))
            {
                response.ERRORS.Add("Your new password cannot be same as old password");
            }
            else
            {
                UserSecret oldPasswordSecret = Utility.GetUserSecret(profile.CREDENTIAL.SALT, changePasswordRequest.OLD_PASSWORD.Trim());

                if (!oldPasswordSecret.SECRET_HASH.Equals(profile.CREDENTIAL.SECRET_HASH))
                {
                    response.ERRORS.Add("Your old password is invalid");
                }
                else
                {
                    UserSecret userSecret = Utility.GetUserSecret(null, changePasswordRequest.NEW_PASSWORD.Trim());

                    Credential credential = profile.CREDENTIAL;

                    credential.SECRET_HASH = userSecret.SECRET_HASH;
                    credential.SALT = userSecret.SALT;

                    _store.CREDENTIALS.Update(credential);
                    _store.SaveChanges();

                    response.IS_CHANGED = true;
                }
            }

            return response;
        }

        public Profile GetProfile(string profileID)
        {
            Profile profile = _cache.Get<Profile>(Profile.PROFILE_CACHE_KEY + profileID);

            if (profile == null)
                profile = _store.PROFILE.Find(profileID);

            return profile;
        }

        public string GenerateOTP(string profileID)
        {
            string otp = Utility.GetUniqueString(Utility.OTP_LENGTH);

            _cache.Add<string>(string.Concat(OTP_CACHE_KEY, profileID), otp);

            return otp; 
        }

        public bool VerifyProfile(string profileID, string responseOTP)
        {
            string cacheOTP = _cache.Get<string>(string.Concat(OTP_CACHE_KEY, profileID));
            if (cacheOTP == null)
                throw new Exception("Missing OTP from cache");

            bool isVerified = cacheOTP.Equals(responseOTP);
            if (isVerified)
            {
                Profile profile = GetProfile(profileID);
                profile.EMAIL_VERIFIED = true;

                _store.PROFILE.Update(profile);
                _store.SaveChanges();
            }

            return isVerified;
        }

        public bool DeleteProfile(string profileID)
        {
            Profile profile = _store.PROFILE.Where(x => x.ID.Equals(profileID)).FirstOrDefault();

            if (profile == null)
                throw new InvalidOperationException("Profile not found");

            profile.STATUS = Identifiers.Status.DELETED;
            _store.PROFILE.Update(profile);
            return _store.SaveChanges() == 1;
        }
    }
}
