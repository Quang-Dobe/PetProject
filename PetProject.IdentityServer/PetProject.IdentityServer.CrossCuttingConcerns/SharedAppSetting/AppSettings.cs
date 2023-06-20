namespace PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; }

        public Auth Auth { get; }

        public EmailSender EmailSender { get; }

        public string AllowedHosts { get; }
    }
}
