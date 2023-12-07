using Microsoft.Extensions.DependencyInjection;
using PetProject.StoreManagement.CrossCuttingConcerns.OS;
using PetProject.StoreManagement.CrossCuttingConcerns.SharedAppSetting;

namespace PetProject.StoreManagement.CrossCuttingConcerns.Extensions
{
    public static class CrossCuttingConcernsExtensions
    {
        public static IServiceCollection AddCrossCuttingConcerns(this IServiceCollection services)
        {
            services.AddSingleton<AppSettings>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            return services;
        }
    }
}
