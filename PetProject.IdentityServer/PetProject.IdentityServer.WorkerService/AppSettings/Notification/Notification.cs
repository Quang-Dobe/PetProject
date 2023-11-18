namespace PetProject.IdentityServer.WorkerService.AppSettings
{
    public class Notification
    {
        public Notification(IConfiguration configuration)
        {
            Email = new Email(configuration);
            Sms = new Sms(configuration);
        }

        public Email Email { get; set; }

        public Sms Sms { get; set; }
    }
}
