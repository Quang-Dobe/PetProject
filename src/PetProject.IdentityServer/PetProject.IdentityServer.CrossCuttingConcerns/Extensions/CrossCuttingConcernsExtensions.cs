using Microsoft.Extensions.DependencyInjection;
using PetProject.IdentityServer.CrossCuttingConcerns.OS;
using PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting;

namespace PetProject.IdentityServer.CrossCuttingConcerns.Extensions
{
    public static class CrossCuttingConcernsExtensions
    {
        public static IServiceCollection AddCrossCuttingConcerns(this IServiceCollection services)
        {
            services.AddSingleton<AppSettings>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            return services;
        }
    }
}
