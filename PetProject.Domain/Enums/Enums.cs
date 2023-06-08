namespace PetProject.IdentityServer.Enums
{
    public enum UserType
    {
        External = 0,

        Internal = 1
    }

    public enum NumberFormatType
    {
        CommasThousandSeparator_DotDecimal = 0,

        DotThousandSeparator_CommasDecimal = 1
    }

    public enum EmailType
    {
        NewUser = 0,
        ForgotPassword = 1,
        AccountLockout = 2,
        AskQuestions = 3
    }
}