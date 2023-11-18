using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetProject.IdentityServer.CrossCuttingConcerns.OS;
using PetProject.IdentityServer.Domain.Repositories;
using PetProject.IdentityServer.Domain.ThirdPartyServices;
using PetProject.IdentityServer.Domain.ThirdPartyServices.SmsSender;
using System.Diagnostics;

namespace PetProject.IdentityServer.WorkerService.WorkerServices
{
    public class SendSmsWorker : BackgroundService
    {
        private readonly IDateTimeProvider _dataTimeProvider;

        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<SendEmailWorker> _logger;

        private Stopwatch _stopwatch;

        public SendSmsWorker(IDateTimeProvider dateTimeProvider, IServiceProvider serviceProvider, ILogger<SendEmailWorker> logger)
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
                        _logger.LogInformation("[SendSmsWorker] Start sending unsent messages");

                        var smsSender = scoppe.ServiceProvider.GetRequiredService<ISmsSender>();
                        var smsRepository = scoppe.ServiceProvider.GetRequiredService<ISmsRepository>();

                        var unsentMessages = smsRepository.GetAll().Where(x => x.Status == false && x.RetryCount < x.MaxRetryCount).ToList();

                        foreach (var message in unsentMessages)
                        {
                            smsSender.SendSms(message);
                            message.Status = true;
                        }

                        await smsRepository.SaveChangeAsync();
                    }

                    await Task.Delay(5000, stoppingToken);
                }
                catch (Exception ex)
                {
                    LogTrace($"[SendSmsWorker] {ex.Message}");
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
