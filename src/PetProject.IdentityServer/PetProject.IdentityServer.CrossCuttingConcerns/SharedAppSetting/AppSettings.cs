using Microsoft.Extensions.Configuration;

namespace PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting
{
    public class AppSettings
    {
        private IConfiguration _configuration;

        public AppSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            Auth = new Auth(configuration);
            EmailSender = new EmailSender(configuration);
            ConnectionStrings = new ConnectionStrings(configuration);
        }

        public Auth Auth { get; set; }

        public EmailSender EmailSender { get;set; }

        public ConnectionStrings ConnectionStrings { get; set; }

        public string AllowedHosts => _configuration["AllowedHosts"];
    }
}
