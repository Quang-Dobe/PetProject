using PetProject.OrderManagement.CrossCuttingConcerns.Extensions;
using PetProject.OrderManagement.Persistence.Extensions;
using PetProject.OrderManagement.Infrastructure.Extensions;
using PetProject.OrderManagement.Infrastructure.ElasticsearchServer.Extensions;
using PetProject.OrderManagement.Infrastructure.ElasticsearchServer;
using PetProject.OrderManagement.Infrastructure.ElasticsearchServer.Services;
using PetProject.OrderManagement.WorkerService.WorkerServices;

namespace PetProject.OrderManagement.WorkerService.Extensions
{
    public static class WorkerServiceExtensions
    {
        public static IServiceCollection AddWorkerService(this IServiceCollection services, AppSettings.AppSettings appSettings)
        {
            services.AddCrossCuttingConcerns();
            services.AddPersistence(appSettings.ConnectionStrings.OrderManagement, "");
            services.AddInfrastructure("");

            services.AddSyncDataToElasticSearchDbWorker(appSettings);

            return services;
        }

        public static IServiceCollection AddSyncDataToElasticSearchDbWorker(this IServiceCollection services, AppSettings.AppSettings appSettings)
        {
            services.AddSingleton<ElasticSearchConfiguration>(new ElasticSearchConfiguration
            {
                BaseUrl = appSettings.ElasticSettings.BaseUrl,
                UserName = appSettings.ElasticSettings.UserName,
                Password = appSettings.ElasticSettings.Password,
                Certificate = appSettings.ElasticSettings.Certificate,
                DefaultIndex = appSettings.ElasticSettings.DefaultIndex,
            });

            services.AddScoped<IElasticSearchServices, ElasticSearchServices>();

            services.AddElasticsearchServer(
                appSettings.ElasticSettings.BaseUrl, 
                appSettings.ElasticSettings.UserName, 
                appSettings.ElasticSettings.Password, 
                appSettings.ElasticSettings.Certificate, 
                appSettings.ElasticSettings.DefaultIndex
            );

            services.AddHostedService<SyncDataToElasticSearchDbWorker>();

            return services;
        }
    }
}
