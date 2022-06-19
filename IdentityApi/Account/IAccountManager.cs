using System.Collections.Generic;
using IdentityApi.DataModels;
using IdentityApi.RequestModels;
using IdentityApi.ResponseModels;

namespace IdentityApi.Account;

public interface IAccountManager
{
    // CURD operation methods
    public Profile GetProfile(string profileID);
    public Person GetPerson(string profileID);
    public List<ProfileCardResponse> GetProfiles();

    public ProfileSaveStatus CreateProfile(Profile profile);

    //public void UpdateProfile(Profile profile);
    public bool DeleteProfile(string profileID);
    public ChangeRequestResponse ChangePassword(string profileID, ChangePasswordRequest changePasswordRequest);
    public string GenerateOTP(string profile);
    public bool VerifyProfile(string profile, string responseOTP);
}
