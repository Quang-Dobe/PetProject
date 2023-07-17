namespace PetProject.IdentityServer.WorkerService.AppSettings
{
    public class Notification
    {
        public Notification(IConfiguration configuration)
        {
            Email = new Email(configuration);
        }

        public Email Email { get; set; }
    }
}
