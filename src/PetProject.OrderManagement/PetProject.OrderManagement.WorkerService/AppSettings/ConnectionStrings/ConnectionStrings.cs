namespace PetProject.OrderManagement.WorkerService.AppSettings
{
    public class ConnectionStrings
    {
        public ConnectionStrings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        public string OrderManagement => _configuration["ConnectionStrings:OrderManagement"] ?? "";

        public string Identity => _configuration["ConnectionStrings:Identity"] ?? "";
    }
}
