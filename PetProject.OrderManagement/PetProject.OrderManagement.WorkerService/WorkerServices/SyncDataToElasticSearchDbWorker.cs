using PetProject.OrderManagement.Infrastructure.ElasticsearchServer.Services;
using PetProject.OrderManagement.WorkerService.WorkerServices.BaseService;
using PetProject.OrderManagement.Domain.Services.BaseService;
using PetProject.OrderManagement.Domain.Entities.BaseEntity;
using PetProject.OrderManagement.CrossCuttingConcerns.OS;
using PetProject.OrderManagement.Domain.Repositories;
using PetProject.OrderManagement.Persistence;
using System.Diagnostics;
using System.Reflection;

namespace PetProject.OrderManagement.WorkerService.WorkerServices
{
    public class SyncDataToElasticSearchDbWorker : CronJobBackgroundService
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IExternalRepoService _externalRepoService;

        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<SyncDataToElasticSearchDbWorker> _logger;

        private Stopwatch _stopwatch;

        public SyncDataToElasticSearchDbWorker(IDateTimeProvider dateTimeProvider, IServiceProvider serviceProvider, IExternalRepoService externalRepoService, ILogger<SyncDataToElasticSearchDbWorker> logger)
        {
            _dateTimeProvider = dateTimeProvider;
            _externalRepoService = externalRepoService;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _stopwatch = Stopwatch.StartNew();

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        _logger.LogInformation("[SyncDataToElasticSearchDbWorker] Start to sync data to ElasticSearch Db");

                        var elasticSearchServices = scope.ServiceProvider.GetRequiredService<IElasticSearchServices>();

                        foreach (Type type in
                            Assembly.GetAssembly(typeof(IBaseRepository<BaseEntity<Guid>>)).GetTypes()
                            .Where(myType => myType.IsClass && myType.GetInterfaces().Length > 1 && myType.GetInterface("IBaseRepository`1") != null))
                        {
                            var context = scope.ServiceProvider.GetService<OrderManagementDbContext>();
                            var methodInfo_GetAll = type.GetMethod("GetAll");
                            var resultData = methodInfo_GetAll?.Invoke(Activator.CreateInstance(type, new object[] { context, _dateTimeProvider, _externalRepoService }), null) as IQueryable<BaseEntity<Guid>>;

                            var unsentSyncData = resultData?.Where(x => !x.IsSync).ToList();

                            foreach (var data in unsentSyncData)
                            {
                                var isExist = await elasticSearchServices.CheckExistAsync(data);
                                if (!isExist)
                                {
                                    await elasticSearchServices.CreateAsync(data);
                                }
                                else
                                {
                                    // Need to update this...
                                    var updatedSuccessfully = await elasticSearchServices.UpdateAsync(data);

                                    if (updatedSuccessfully)
                                    {
                                        data.IsSync = true;
                                    }
                                    else
                                    {
                                        data.IsSync = false;
                                    }
                                }
                            }

                            var methodInfo_SaveChanges = type.GetMethod("SaveChanges");
                            methodInfo_SaveChanges.Invoke(Activator.CreateInstance(type, new object[] { context, _dateTimeProvider, _externalRepoService }), null);
                        }

                        await Task.Delay(5000, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    LogTrace($"[SyncDataToElasticSearchDbWorker] {ex.Message}");
                    await Task.Delay(15000, stoppingToken);
                }

                _stopwatch.Stop();
            }
        }

        #region Private methods

        private void LogTrace(string? message)
        {
            _stopwatch.Stop();
            _logger.LogInformation(string.Format(" At {0}. Time spent {1} ", _dateTimeProvider.Now, _stopwatch.Elapsed));
            _logger.LogInformation(string.Format(" Message: {0} ", message));
        }

        #endregion
    }
}
