using Microsoft.Extensions.DependencyInjection;
using PetProject.IdentityServer.CrossCuttingConcerns.OS;

namespace PetProject.IdentityServer.CrossCuttingConcerns.Extensions
{
    public static class CrossCuttingConcernsExtensions
    {
        public static IServiceCollection AddCrossCuttingConcerns(this IServiceCollection services)
        {
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            return services;
        }
    }
}
