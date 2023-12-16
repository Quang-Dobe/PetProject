namespace PetProject.StoreManagement.WorkerService
{
    public class AppSettings
    {
        public AppSettings(IConfiguration configuration)
        {
            ConnectionStrings = new ConnectionStrings(configuration);
        }

        public ConnectionStrings ConnectionStrings { get; set; }
    }
}
