using PetProject.StoreManagement.CrossCuttingConcerns.OS;
using PetProject.StoreManagement.Domain.Entities;
using PetProject.StoreManagement.Domain.Repositories;
using PetProject.StoreManagement.Domain.ThirdPartyServices.MQBroker;
using PetProject.StoreManagement.WorkerService.WorkerServices.BaseService;
using StackExchange.Redis;
using System.Diagnostics;

namespace PetProject.StoreManagement.WorkerService.WorkerServices
{
    public class SyncDataToMQWorker : CronJobBackgroundService
    {
        private readonly IDateTimeProvider _dataTimeProvider;

        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<SyncDataToMQWorker> _logger;

        private Stopwatch _stopwatch;

        public SyncDataToMQWorker(IDateTimeProvider dateTimeProvider, IServiceProvider serviceProvider, ILogger<SyncDataToMQWorker> logger)
        {
            _dataTimeProvider = dateTimeProvider;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _stopwatch = Stopwatch.StartNew();
                _logger.LogInformation("[SyncDataToMQWorker] Start to sync data to Message Queue");

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mqService = scope.ServiceProvider.GetRequiredService<IMessageQueueBroker>();
                        var storageRepository = scope.ServiceProvider.GetRequiredService<IStorageRepository>();

                        var unSyncedData = storageRepository.GetAll().Where(x => !x.IsSync).ToList();

                        foreach (var data in unSyncedData)
                        {
                            mqService.SendMessage(data, nameof(Storage));
                        }
                    }

                    await Task.Delay(5000, stoppingToken);
                }
                catch (Exception ex)
                {
                    LogTrace($"[SyncDataToMQWorker] {ex.Message}");
                    await Task.Delay(15000, stoppingToken);
                }

                _stopwatch.Stop();
                _logger.LogInformation("[SyncDataToMQWorker] End to sync data to Message Queue");
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
