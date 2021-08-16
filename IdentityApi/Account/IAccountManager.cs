using System;
using IdentityApi.DataModels;

namespace IdentityApi.Account
{
    public interface IAccountManager
    {
        // CURD operation methods
        //public Profile GetProfile(string profileID);
        //public List<ProfileCardResponse> GetProfiles();
        public ProfileSaveStatus CreateProfile(Profile profile);
        //public void UpdateProfile(Profile profile);
        //public void DeleteProfile(string profileID);
        //public ChangeRequestResponse ChangePassword(string profileID, ChangePasswordRequest changePasswordRequest);
    }
}
