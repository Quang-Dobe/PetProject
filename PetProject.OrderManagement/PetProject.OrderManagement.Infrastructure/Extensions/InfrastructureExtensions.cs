using Microsoft.Extensions.DependencyInjection;
using PetProject.OrderManagement.Domain.Services.BaseService;
using PetProject.OrderManagement.Domain.ThirdPartyServices.Caching;
using PetProject.OrderManagement.Infrastructure.CachingService;
using PetProject.OrderManagement.Infrastructure.ElasticsearchServer.Services;

namespace PetProject.OrderManagement.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string cachingConnectionString)
        {
            services.AddCaching(cachingConnectionString);

            return services;
        }

        public static IServiceCollection AddCaching(this IServiceCollection services, string cachingConnectionString)
        {
            services.AddStackExchangeRedisCache(options => options.Configuration = cachingConnectionString);

            services.AddScoped<ICaching, Caching>();

            return services;
        }
    }
}
