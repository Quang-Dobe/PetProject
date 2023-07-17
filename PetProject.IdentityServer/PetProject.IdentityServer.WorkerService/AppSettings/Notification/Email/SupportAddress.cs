namespace PetProject.IdentityServer.WorkerService.AppSettings
{
    public class SupportAddress
    {
        private readonly IConfiguration _configuration;

        public SupportAddress(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string FromEmail => _configuration["Notification:Email:SupportAddress:FromEmail"];

        public string FromName => _configuration["Notification:Email:SupportAddress:FromName"];
    }
}