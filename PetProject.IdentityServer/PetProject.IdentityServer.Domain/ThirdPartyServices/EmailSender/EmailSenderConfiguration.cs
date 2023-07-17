namespace PetProject.IdentityServer.Domain.ThirdPartyServices.EmailSender
{
    public class EmailSenderConfiguration
    {
        public string SmtpHost { get; set; }

        public int SmtpPort { get; set; }

        public bool SmtpEnableSsl { get; set; }

        public string SmtpUserName { get; set; }

        public string SmtpPassword { get; set; }
    }
}
