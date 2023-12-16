namespace PetProject.StoreManagement.WorkerService
{
    public class ConnectionStrings
    {
        public ConnectionStrings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        public string StoreManagement => _configuration["ConnectionStrings:StoreManagement"] ?? "";

        public string Identity => _configuration["ConnectionStrings:Identity"] ?? "";
    }
}
