using Microsoft.Extensions.Configuration;

namespace PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting
{
    public class EmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string FromName => _configuration["EmailSender:FromName"];

        public string FromEmail => _configuration["EmailSender:FromEmail"];

        public string AccountLockoutTitle => _configuration["EmailSender:AccountLockoutTitle"];

        public string RegisterNewUserTitle => _configuration["EmailSender:RegisterNewUserTitle"];
    }
}
