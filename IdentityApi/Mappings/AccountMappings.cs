using System;
namespace IdentityApi.Mappings
{
    public class AccountMappings
    {
        public const string ENDPOINT_ROUTE = "account";
        public const string VALIDATE_LOG_IN = "validate-log-in";
        public const string CHANGE_PASSWORD = "change-password";
        public const string GET_PROFILES = "get-profiles";
        public const string CREATE_NEW_PROFILE = "create-new-profile";
        public const string CHECK_PWNED_PASSWORD = "check-pwned-password";
        public const string SEND_VERIFICATION_CODE = "send-verification-code";
        public const string VERIFY_PROFILE = "verify-profile";
        public const string DELETE_PROFILE = "delete-profile";
    }
}
