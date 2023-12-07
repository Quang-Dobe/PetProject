namespace PetProject.IdentityServer.WorkerService.AppSettings
{
    public class Email
    {
        private readonly IConfiguration _configuration;

        public Email(IConfiguration configuration)
        {
            _configuration = configuration;
            AdminAddress = new AdminAddress(configuration);
            SupportAddress = new SupportAddress(configuration);
        }

        public AdminAddress AdminAddress { get; set; }

        public SupportAddress SupportAddress { get; set; }

        public string SmtpServerHost => _configuration["Notification:Email:SmtpServerHost"] ?? "smtp.gmail.com";

        public int SmtpServerPort => Convert.ToInt32(_configuration["Notification:Email:SmtpServerPort"] ?? "465");

        public string SmtpServerUserName => _configuration["Notification:Email:SmtpServerUserName"] ?? "";

        public string SmtpServerPassword => _configuration["Notification:Email:SmtpServerPassword"] ?? "";

        public bool SmtpServerEnableSsl => Convert.ToBoolean(_configuration["Notification:Email:SmtpServerEnableSsl"] ?? "false");

        public bool EnableEmailNotification => Convert.ToBoolean(_configuration["Notification:Email:EnableEmailNotification"] ?? "false");
    }
}