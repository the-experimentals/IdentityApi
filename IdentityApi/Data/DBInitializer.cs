using System;
using IdentityApi.Account;
using IdentityApi.DataModels;

namespace IdentityApi.Data
{
    public class DBInitializer
    {
        private readonly IAccountManager _accountManager;
        public DBInitializer(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public void Initialize()
        {
            SeedAdmin();
        }

        /// <summary>
        /// Seeds default admin account for new database.
        /// </summary>
        private void SeedAdmin()
        {
            Profile newAdmin = new()
            {
                ID = Profile.ADMIN_GUID,
                NAME = "admin",
                NEW = true,
                CREATED_BY = "system",
                EMAIL = "test@test.com"
            };

            newAdmin.CREDENTIAL = new()
            {
                USERNAME = "system",
                PASSWORD = "DONOTSHARE"
            };

            //_accountManager.CreateProfile(newAdmin);
        }
    }
}
