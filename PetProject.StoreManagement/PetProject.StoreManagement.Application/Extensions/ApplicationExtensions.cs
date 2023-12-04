using Microsoft.Extensions.DependencyInjection;
using PetProject.StoreManagement.Domain.ThirdPartyServices.ExternalRepoService;
using PetProject.StoreManagement.Infrastructure.ExternalRepoService;

namespace PetProject.StoreManagement.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IExternalRepoService, ExternalRepoService>();

            return services;
        }
    }
}
