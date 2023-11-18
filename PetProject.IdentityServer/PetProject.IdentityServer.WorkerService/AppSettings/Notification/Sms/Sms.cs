namespace PetProject.IdentityServer.WorkerService.AppSettings
{
    public class Sms
    {
        private readonly IConfiguration _configuration;

        public Sms(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConnectionString => _configuration["Notification:Sms:ConnectionString"] ?? "";

        public string FromPhoneNumber => _configuration["Notification:Sms:FromPhoneNumber"] ?? "";
    }
}
