using System;
using System.Collections.Generic;
using System.Linq;
using IdentityApi.DataModels;
using IdentityApi.Identifiers;
using IdentityApi.RequestModels;
using IdentityApi.ResponseModels;
using IdentityApi.Services.SQLServer;
using IdentityApi.Utilities;

namespace IdentityApi.Account;

public class AccountManager : IAccountManager
{
    private readonly TMCache _cache;
    private readonly IdentityStore _store;
    private readonly string OTP_CACHE_KEY = "CACHE_OTP_";

    public AccountManager(IdentityStore store, TMCache cache)
    {
        _store = store;
        _cache = cache;
    }

    public ProfileSaveStatus CreateProfile(Profile profile)
    {
        ProfileSaveStatus status = new();

        var credential = profile.CREDENTIAL;

        if (profile.NEW)
        {
            // check credentials for new profile already exist in records.
            if (!_store.CREDENTIALS.Any(c => c.USERNAME.Equals(credential.USERNAME.ToLower().Trim())))
            {
                var person = profile.PERSON;

                if (string.IsNullOrWhiteSpace(profile.ID))
                {
                    profile.ID = Guid.NewGuid().ToString();
                }

                if (person != null)
                {
                    profile.NAME = person.FIRST_NAME + " " + person.LAST_NAME;
                }

                profile.CREATED_ON = DateTime.Now;
            }
            else
            {
                status.IS_SAVED = false;
                status.ERRORS.Add("Username already exist. Please use other username to register.");
                return status;
            }
        }

        // Begin transaction for creating/updating profile in database.
        using var transaction = _store.Database.BeginTransaction();

        try
        {
            if (profile.NEW)
            {
                _store.PROFILE.Add(profile);

                credential.PROFILE_ID = profile.ID;
                CreateCredentials(credential);

                var p = profile.PERSON;
                p.ID = Guid.NewGuid().ToString();
                p.PROFILE_ID = profile.ID;

                _store.PERSON.Add(profile.PERSON);
                _store.SaveChanges();
            }

            // else
            // {
            //     _store.PROFILE.Update(profile);
            // }

            _store.SaveChanges();

            transaction.Commit();
            status.IS_SAVED = true;
            status.PROFILE_ID = profile.ID;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine(ex.StackTrace);
            throw new InvalidOperationException(ex.Message, ex.InnerException);
        }

        return status;
    }

    public List<ProfileCardResponse> GetProfiles()
    {
        var profiles = (from profile in _store.PROFILE
                        join credential in _store.CREDENTIALS
                            on profile.ID equals credential.PROFILE_ID
                        where credential.USERNAME != "system" && profile.STATUS != Status.DELETED
                        select new { dataProfile = profile, dataCredential = credential }).ToList();

        List<ProfileCardResponse> profileViews = new();

        if (profiles.Any())
        {
            foreach (var profile in profiles)
            {
                profileViews.Add(new ProfileCardResponse
                {
                    PROFILE_ID = profile.dataProfile.ID,
                    USERNAME = profile.dataCredential.USERNAME,
                    NAME = profile.dataProfile.NAME,
                    LOCKED = profile.dataProfile.LOCKED,
                    INITIALS = GetNameInitials(profile.dataProfile.NAME)
                });
            }
        }

        return profileViews;
    }

    public ChangeRequestResponse ChangePassword(string profileID, ChangePasswordRequest changePasswordRequest)
    {
        var profile = GetProfile(profileID);

        ChangeRequestResponse response = new();

        var newPasswordSecret =
            Utility.GetUserSecret(profile.CREDENTIAL.SALT, changePasswordRequest.NEW_PASSWORD.Trim());

        if (newPasswordSecret.SECRET_HASH.Equals(profile.CREDENTIAL.SECRET_HASH))
        {
            response.ERRORS.Add("Your new password cannot be same as old password");
        }
        else
        {
            var oldPasswordSecret =
                Utility.GetUserSecret(profile.CREDENTIAL.SALT, changePasswordRequest.OLD_PASSWORD.Trim());

            if (!oldPasswordSecret.SECRET_HASH.Equals(profile.CREDENTIAL.SECRET_HASH))
            {
                response.ERRORS.Add("Your old password is invalid");
            }
            else
            {
                var userSecret = Utility.GetUserSecret(null, changePasswordRequest.NEW_PASSWORD.Trim());

                var credential = profile.CREDENTIAL;

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
        var profile = _cache.Get<Profile>(Profile.PROFILE_CACHE_KEY + profileID);

        if (profile == null)
        {
            var dbProfile = (from p in _store.PROFILE
                             join person in _store.PERSON
                                 on p.ID equals person.PROFILE_ID
                             where p.ID == profileID
                             select new { dataProfile = p, dataPerson = person }).FirstOrDefault();

            if (dbProfile != null)
            {
                profile = dbProfile.dataProfile;
                profile.PERSON = dbProfile.dataPerson;
                _cache.Add(Profile.PROFILE_CACHE_KEY + profileID, profile);
            }
        }


        return profile;
    }

    public string GenerateOTP(string profileID)
    {
        var otp = Utility.GetUniqueString(Utility.OTP_LENGTH);

        _cache.Add(string.Concat(OTP_CACHE_KEY, profileID), otp);

        return otp;
    }

    public bool VerifyProfile(string profileID, string responseOTP)
    {
        var cacheOTP = _cache.Get<string>(string.Concat(OTP_CACHE_KEY, profileID));
        if (cacheOTP == null)
        {
            throw new Exception("Missing OTP from cache");
        }

        var isVerified = cacheOTP.Equals(responseOTP);
        if (isVerified)
        {
            var profile = GetProfile(profileID);
            profile.EMAIL_VERIFIED = true;

            _store.PROFILE.Update(profile);
            _store.SaveChanges();
        }

        return isVerified;
    }

    public bool DeleteProfile(string profileID)
    {
        var profile = _store.PROFILE.Where(x => x.ID.Equals(profileID)).FirstOrDefault();

        if (profile == null || profile.ID.Equals(Profile.ADMIN_GUID))
        {
            throw new InvalidOperationException("Profile not found");
        }

        profile.STATUS = Status.DELETED;

        _store.PROFILE.Update(profile);

        return _store.SaveChanges() > 0;
    }


    public Person GetPerson(string profileID)
    {
        throw new NotImplementedException();
    }

    private string GetNameInitials(string name)
    {
        var initials = "";

        if (name.Length == 0)
        {
            return "";
        }

        var splitName = name.Split(" ");

        foreach (var sp in splitName)
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

        var existingCredential = _store.CREDENTIALS.FirstOrDefault(c => c.PROFILE_ID.Equals(credential.PROFILE_ID));
        if (existingCredential == null)
        {
            // get credential type opted
            // for now username credential supported

            var userSecret = Utility.GetUserSecret(null, credential.PASSWORD);

            Credential newCredential = new()
            {
                ID = Guid.NewGuid().ToString(),
                USERNAME = credential.USERNAME.Trim().ToLower(),
                SECRET_HASH = userSecret.SECRET_HASH,
                PROFILE_ID = credential.PROFILE_ID,
                SALT = userSecret.SALT
            };

            _store.CREDENTIALS.Add(newCredential);
        }
    }
}
