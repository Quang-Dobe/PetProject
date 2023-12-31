﻿namespace PetProject.IdentityServer.Domain.Constants
{
    public static class Constants
    {
        public static class GrantType
        {
            public const string Password = "password";

            public const string RefreshToken = "refresh_token";

            public const string ClientCredential = "client_credential";
        }

        public static int AccountLockoutTimeSpan = 5;

        public static int GeneratedPasswordLength = 12;
    }
}
