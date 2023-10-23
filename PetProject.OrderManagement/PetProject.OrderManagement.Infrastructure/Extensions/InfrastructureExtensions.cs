using Microsoft.Extensions.DependencyInjection;
using PetProject.OrderManagement.Domain.Services.BaseService;
using PetProject.OrderManagement.Infrastructure.ElasticsearchServer.Services;

namespace PetProject.OrderManagement.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            //services.AddElasticSearchServer();

            return services;
        }

        public static IServiceCollection AddElasticSearchServer(this IServiceCollection services)
        {
            services.AddSingleton<IExternalRepoService, ElasticsearchServices>();
            return services;
        }
    }
}
