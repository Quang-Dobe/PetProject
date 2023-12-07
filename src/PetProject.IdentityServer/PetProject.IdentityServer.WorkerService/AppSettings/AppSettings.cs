namespace PetProject.IdentityServer.WorkerService.AppSettings
{
    public class AppSettings
    {
        public AppSettings(IConfiguration configuration) 
        {
            Notification = new Notification(configuration);
            ConnectionStrings = new ConnectionStrings(configuration);
        }

        public Notification Notification { get; set; }

        public ConnectionStrings ConnectionStrings { get; set; }
    }
}
