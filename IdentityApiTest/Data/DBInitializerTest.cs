using IdentityApi.Data;
using IdentityApiTest.Mockings;
using IdentityApiTest.Ordering;
using Xunit;

namespace IdentityApiTest.Data;

[TestCaseOrderer("IdentityApiTest.Ordering.PriorityOrder", "IdentityApiTest")]
public class DBInitializerTest : IClassFixture<AccountManagerMock>
{
    private readonly DBInitializer _dBInitializer;

    public DBInitializerTest(AccountManagerMock accountManagerMock)
    {
        _dBInitializer = new DBInitializer(accountManagerMock._accountManager);
    }

    [Fact(DisplayName = "Test seeding default admin profile.")]
    [Priority(1)]
    public void TestSeedAdmin()
    {
        var result = _dBInitializer.SeedAdmin();
        Assert.True(result.IS_SAVED);
    }
}
