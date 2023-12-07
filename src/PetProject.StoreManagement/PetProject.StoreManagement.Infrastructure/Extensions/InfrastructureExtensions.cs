using Microsoft.Extensions.DependencyInjection;
using PetProject.StoreManagement.Domain.ThirdPartyServices.Caching;
using PetProject.StoreManagement.Infrastructure.CachingService;

namespace PetProject.StoreManagement.Infrastructure.Extensions
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
