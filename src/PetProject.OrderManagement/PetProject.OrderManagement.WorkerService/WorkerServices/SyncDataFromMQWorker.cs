using PetProject.OrderManagement.CrossCuttingConcerns.OS;
using PetProject.OrderManagement.Domain.Entities;
using PetProject.OrderManagement.Domain.Entities.BaseEntity;
using PetProject.OrderManagement.Domain.Repositories;
using PetProject.OrderManagement.Domain.Services.BaseService;
using PetProject.OrderManagement.Domain.ThirdPartyServices.MQBroker;
using PetProject.OrderManagement.Persistence;
using PetProject.OrderManagement.WorkerService.WorkerServices.BaseService;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PetProject.OrderManagement.WorkerService.WorkerServices
{
    public class SyncDataFromMQWorker : CronJobBackgroundService
    {
        private readonly IDateTimeProvider _dataTimeProvider;

        private readonly IServiceProvider _serviceProvider;

        private readonly IExternalRepoService _externalRepoService;

        private readonly ILogger<SyncDataFromMQWorker> _logger;

        private Stopwatch _stopwatch;

        public SyncDataFromMQWorker(IDateTimeProvider dateTimeProvider, IServiceProvider serviceProvider, IExternalRepoService externalRepoService, ILogger<SyncDataFromMQWorker> logger)
        {
            _dataTimeProvider = dateTimeProvider;
            _serviceProvider = serviceProvider;
            _externalRepoService = externalRepoService;
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _stopwatch = Stopwatch.StartNew();
                _logger.LogInformation("[SyncDataFromMQWorker] Start to sync data from Message Queue");

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mqService = scope.ServiceProvider.GetRequiredService<IMessageQueueBroker>();

                        var portData = new List<Port>();
                        mqService.ReceiveMessage(nameof(Port), (sender, message) =>
                        {
                            var entity = JsonSerializer.Deserialize<Port>(message);
                            if (entity != null)
                            {
                                portData.Add(entity);
                            }
                        });

                        var userData = new List<User>();
                        mqService.ReceiveMessage(nameof(User), (sender, message) =>
                        {
                            var entity = JsonSerializer.Deserialize<User>(message);
                            if (entity != null)
                            {
                                userData.Add(entity);
                            }
                        });

                        var clientData = new List<Client>();
                        mqService.ReceiveMessage(nameof(Client), (sender, message) =>
                        {
                            var entity = JsonSerializer.Deserialize<Client>(message);
                            if (entity != null)
                            {
                                clientData.Add(entity);
                            }
                        });

                        var companyData = new List<Company>();
                        mqService.ReceiveMessage(nameof(Company), (sender, message) =>
                        {
                            var entity = JsonSerializer.Deserialize<Company>(message);
                            if (entity != null)
                            {
                                companyData.Add(entity);
                            }
                        });

                        var storageData = new List<Storage>();
                        mqService.ReceiveMessage(nameof(Storage), (sender, message) =>
                        {
                            var entity = JsonSerializer.Deserialize<Storage>(message);
                            if (entity != null)
                            {
                                storageData.Add(entity);
                            }
                        });

                        var containerData = new List<Container>();
                        mqService.ReceiveMessage(nameof(Container), (sender, message) =>
                        {
                            var entity = JsonSerializer.Deserialize<Container>(message);
                            if (entity != null)
                            {
                                containerData.Add(entity);
                            }
                        });

                        var organisationData = new List<Organisation>();
                        mqService.ReceiveMessage(nameof(Organisation), (sender, message) =>
                        {
                            var entity = JsonSerializer.Deserialize<Organisation>(message);
                            if (entity != null)
                            {
                                organisationData.Add(entity);
                            }
                        });

                        var productData = new List<Product>();
                        mqService.ReceiveMessage(nameof(Product), (sender, message) =>
                        {
                            var entity = JsonSerializer.Deserialize<Product>(message);
                            if (entity != null)
                            {
                                productData.Add(entity);
                            }
                        });

                        await UpdateDatabaseAsync(portData);
                        await UpdateDatabaseAsync(userData);
                        await UpdateDatabaseAsync(clientData);
                        await UpdateDatabaseAsync(companyData);
                        await UpdateDatabaseAsync(storageData);
                        await UpdateDatabaseAsync(containerData);
                        await UpdateDatabaseAsync(organisationData);
                    }

                    await Task.Delay(5000, stoppingToken);
                }
                catch (Exception ex)
                {
                    LogTrace($"[SyncDataFromMQWorker] {ex.Message}");
                    await Task.Delay(15000, stoppingToken);
                }

                _stopwatch.Stop();
                _logger.LogInformation("[SyncDataFromMQWorker] Start to sync data from Message Queue");
            }
        }

        #region Private methods

        private Task UpdateDatabaseAsync<T>(List<T> syncedData) where T : AggregateEntity<Guid>
        {
            try
            {
                var syncedDataGuids = syncedData.Select(x => x.Id).ToList();
                using (var scope = _serviceProvider.CreateScope())
                {
                    // Get the exact type that mapping to Entity in DbContext
                    // If it's null, just don't care...
                    var type = Assembly.GetAssembly(typeof(IBaseRepository<BaseEntity<Guid>>)).GetTypes()
                                .Where(myType => myType.IsClass && myType.GetInterfaces().Length > 1 && myType.GetInterface("IBaseRepository`1") != null && myType.Name.Contains(nameof(T))).FirstOrDefault();
                    if (type == null)
                    {
                        return Task.CompletedTask;
                    }

                    // Invoke GetAll() to get data based on type of Entity
                    var context = scope.ServiceProvider.GetService<OrderManagementDbContext>();
                    var contextInstance = Activator.CreateInstance(type, new object[] { context, _dataTimeProvider, _externalRepoService });
                    var methodInfo_GetAll = type?.GetMethod("GetAll");
                    var resultData = methodInfo_GetAll?.Invoke(contextInstance, null) as IQueryable<BaseEntity<Guid>>;
                    var currentData = resultData.Where(x => syncedDataGuids.Contains(x.Id));

                    // Get all valid properties that both Entity in current DbContext and Synced data have (Except Id and Version column)
                    var syncedDataProperties = typeof(T).GetProperties();
                    var currentDataProperties = type?.GetProperties();
                    var matchingProperties = currentDataProperties?.Intersect(syncedDataProperties).Except(syncedDataProperties.Where(x => x.Name.Contains("Id") || x.Name.Contains("Version")));
                    
                    // Foreach Entity in current DbContext
                    // If it's included in Synced data -> Update all fields
                    foreach (var data in currentData)
                    {
                        // Add more code here to update existing data...
                        var matchingSyncedData = syncedData.FirstOrDefault(x => x.Id == data.Id);
                        if (matchingSyncedData != null)
                        {
                            foreach (var propertyInfo in matchingProperties)
                            {
                                propertyInfo.SetValue(data, propertyInfo.GetValue(matchingSyncedData));
                            }
                        }
                    }

                    // SaveChanges
                    var methodInfo_SaveChanges = type?.GetMethod("SaveChanges");
                    methodInfo_SaveChanges.Invoke(contextInstance, null);

                    return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        private void LogTrace(string? message)
        {
            _stopwatch.Stop();
            _logger.LogInformation(string.Format(" At {0}. Time spent {1} ", _dataTimeProvider.Now, _stopwatch.Elapsed));
            _logger.LogInformation(string.Format(" Message: {0} ", message));
        }

        #endregion
    }
}
