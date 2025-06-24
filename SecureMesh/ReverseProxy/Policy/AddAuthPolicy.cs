namespace SecureMesh.ReverseProxy.Policy;

/// <summary>
/// Template Policy of ReverseProxy Routes
/// </summary>
public static class AddAuthPolicy
{
    public static class Policies
    {
        public const string BasicPolicy = "BasicPolicy";
        public const string AdminPolicy = "AdminPolicy";
        public const string CreatorPolicy = "CreatorPolicy";

        //rate limiting Policy
        public const string LoginPolicy = "login-rs";
        public const string LogOutPolicy = "logout";
        public const string RegisterPolicy = "register";
        public const string PasswordUpdateLimit = "password-update";
        public const string ForgetPasswordPolicy = "forget-password";
        public const string ResetPasswordPolicy = "reset-password";
        public const string UpdateEmailPolicy = "update-email";
        public const string UpdateCredentials = "update-credentials";
        public const string UpdateAnyCredentials = "update-any-credentials";
        public const string VerifyEmailPolicy = "verify-email";
        public const string VerifyNewEmailPolicy = "verify-new-email";
        public const string RemoveUserPolicy = "remove-user";

    }
}

