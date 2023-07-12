namespace PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting
{
    public class EmailSender
    {
        public string FromName { get; }

        public string FromEmail { get; }

        public string AccountLockoutTitle { get; }

        public string RegisterNewUserTitle { get; }
    }
}
