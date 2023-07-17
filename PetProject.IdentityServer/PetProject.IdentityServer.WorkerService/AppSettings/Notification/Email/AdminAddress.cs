namespace PetProject.IdentityServer.WorkerService.AppSettings
{
    public class AdminAddress
    {
        private readonly IConfiguration _configuration;

        public AdminAddress(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string FromEmail => _configuration["Notification:Email:AdminAddress:FromEmail"];

        public string FromName => _configuration["Notification:Email:AdminAddress:FromName"];
    }
}
