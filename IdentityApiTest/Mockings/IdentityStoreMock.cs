using System;
using IdentityApi.Account;
using IdentityApi.DataModels;
using IdentityApi.Services.SQLServer;
using IdentityApi.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IdentityApiTest.Mockings
{
    public class IdentityStoreMock
    {
        public IdentityStore _store;
        public string testProfileGuid = Guid.NewGuid().ToString();

        public IdentityStoreMock()
        {
            DbContextOptions<IdentityStore> options = new DbContextOptionsBuilder<IdentityStore>()
                              .UseInMemoryDatabase(databaseName: "testDB")
                              .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                              .Options;

            _store = new IdentityStore(options);

            UserSecret userSecret = Utility.GetUserSecret(null, "defaultTest");

            Credential credential = new()
            {
                ID = Guid.NewGuid().ToString(),
                USERNAME = "default",
                PROFILE_ID = testProfileGuid,
                SALT = userSecret.SALT,
                SECRET_HASH = userSecret.SECRET_HASH
            };

            _store.PROFILE.Add(new Profile
            {
                ID = testProfileGuid,
                NAME = "default test user",
                NEW = true,
                CREATED_BY = "system",
                EMAIL = "test@test.com",
                CREDENTIAL = credential
            });

            _store.CREDENTIALS.Add(credential);

            _store.SaveChanges();
        }
    }
}
