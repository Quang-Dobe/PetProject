using PetProject.IdentityServer.Domain.ThirdPartyServices.EmailSender;
using PetProject.IdentityServer.Domain.ThirdPartyServices;
using PetProject.IdentityServer.WorkerService.WorkerServices;
using PetProject.IdentityServer.Infrastructure.EmailSenderService;
using PetProject.IdentityServer.Persistence.Extensions;
using PetProject.IdentityServer.CrossCuttingConcerns.Extensions;
using PetProject.IdentityServer.Infrastructure.Extensions;

namespace PetProject.IdentityServer.WorkerService.Extensions
{
    public static class WorkerServiceExtensions
    {
        public static IServiceCollection AddWorkerService(this IServiceCollection services, AppSettings.AppSettings appSettings)
        {
            services.AddCrossCuttingConcerns();
            services.AddPersistence(appSettings.ConnectionStrings.Identity, "");
            services.AddInfrastructure();

            services.AddSendEmailWorker(appSettings);

            return services;
        }

        public static IServiceCollection AddSendEmailWorker(this IServiceCollection services, AppSettings.AppSettings appSettings)
        {
            services.AddSingleton<EmailSenderConfiguration>(new EmailSenderConfiguration()
            {
                SmtpHost = appSettings.Notification.Email.SmtpServerHost,
                SmtpPort = appSettings.Notification.Email.SmtpServerPort,
                SmtpEnableSsl = appSettings.Notification.Email.SmtpServerEnableSsl,
                SmtpUserName = appSettings.Notification.Email.SmtpServerUserName,
                SmtpPassword = appSettings.Notification.Email.SmtpServerPassword,
            });

            services.AddSingleton<IEmailSender, EmailSender>();

            services.AddHostedService<SendEmailWorker>();

            return services;
        }
    }
}
