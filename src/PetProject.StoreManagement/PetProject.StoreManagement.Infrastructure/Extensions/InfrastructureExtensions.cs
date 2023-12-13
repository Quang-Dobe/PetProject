using Microsoft.Extensions.DependencyInjection;
using PetProject.StoreManagement.Domain.ThirdPartyServices.Caching;
using PetProject.StoreManagement.Domain.ThirdPartyServices.MQBroker;
using PetProject.StoreManagement.Infrastructure.CachingService;
using PetProject.StoreManagement.Infrastructure.MQBroker;

namespace PetProject.StoreManagement.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string cachingConnectionString)
        {
            services.AddCaching(cachingConnectionString);
            services.AddMQBroker();

            return services;
        }

        public static IServiceCollection AddCaching(this IServiceCollection services, string cachingConnectionString)
        {
            services.AddStackExchangeRedisCache(options => options.Configuration = cachingConnectionString);

            services.AddScoped<ICaching, Caching>();

            return services;
        }

        public static IServiceCollection AddMQBroker(this IServiceCollection services)
        {
            services.AddScoped<IMessageQueueBroker, MessageQueueBroker>();

            return services;
        }
    }
}
