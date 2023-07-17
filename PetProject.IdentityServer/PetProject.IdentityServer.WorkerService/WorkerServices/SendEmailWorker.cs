using PetProject.IdentityServer.CrossCuttingConcerns.OS;
using PetProject.IdentityServer.Domain.Repositories;
using PetProject.IdentityServer.Domain.ThirdPartyServices;
using System.Diagnostics;

namespace PetProject.IdentityServer.WorkerService.WorkerServices
{
    public class SendEmailWorker : BackgroundService
    {
        private readonly IDateTimeProvider _dataTimeProvider;

        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<SendEmailWorker> _logger;

        private Stopwatch _stopwatch;

        public SendEmailWorker(IDateTimeProvider dateTimeProvider, IServiceProvider serviceProvider, ILogger<SendEmailWorker> logger)
        {
            _dataTimeProvider = dateTimeProvider;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _stopwatch = Stopwatch.StartNew();

                try
                {
                    using (var scoppe = _serviceProvider.CreateScope())
                    {
                        _logger.LogInformation("[SendEmailWorker] Start sending unsent emails");

                        var emailSender = scoppe.ServiceProvider.GetRequiredService<IEmailSender>();
                        var emailRepository = scoppe.ServiceProvider.GetRequiredService<IEmailRepository>();

                        var unsentEmails = emailRepository.GetAll().Where(x => x.Status == false).ToList();

                        foreach (var email in unsentEmails)
                        {
                            emailSender.SendEmail(email);
                            email.Status = true;
                        }

                        await emailRepository.SaveChangeAsync();
                    }

                    await Task.Delay(5000, stoppingToken);
                }
                catch (Exception ex)
                {
                    LogTrace($"[SendEmailWorker] {ex.Message}");
                    await Task.Delay(15000, stoppingToken);
                }

                _stopwatch.Stop();
            }
        }

        #region Private methods

        private void LogTrace(string? message)
        {
            _stopwatch.Stop();
            _logger.LogInformation(string.Format(" At {0}. Time spent {1} ", _dataTimeProvider.Now, _stopwatch.Elapsed));
            _logger.LogInformation(string.Format(" Message: {0} ", message));
        }

        #endregion
    }
}
