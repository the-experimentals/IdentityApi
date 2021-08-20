using System;
using System.Collections.Generic;
using System.Linq;
using IdentityApi.DataModels;
using IdentityApi.ResponseModels;
using IdentityApi.Services.SQLServer;
using IdentityApi.Utilities;
using Microsoft.EntityFrameworkCore.Storage;

namespace IdentityApi.Account
{
    public class AccountManager : IAccountManager
    {
        private readonly IdentityStore _store;
        public AccountManager(IdentityStore store)
        {
            _store = store;
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
    }
}
