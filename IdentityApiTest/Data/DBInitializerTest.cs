using System;
using IdentityApi.Account;
using IdentityApi.Data;
using IdentityApiTest.Mockings;
using Xunit;

namespace IdentityApiTest.Data
{
    public class DBInitializerTest : IClassFixture<AccountManagerMock>
    {        
        private DBInitializer _dBInitializer;

        public DBInitializerTest(AccountManagerMock accountManagerMock)
        {           
            _dBInitializer = new DBInitializer(accountManagerMock._accountManager);
        }

        [Fact]
        public void TestSeedAdmin()
        {
            ProfileSaveStatus result =_dBInitializer.SeedAdmin();
            Assert.True(result.IS_SAVED);
        }
    }
}
