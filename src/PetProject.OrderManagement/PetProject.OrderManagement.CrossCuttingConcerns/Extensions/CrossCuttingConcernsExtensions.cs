using Microsoft.Extensions.DependencyInjection;
using PetProject.OrderManagement.CrossCuttingConcerns.OS;
using PetProject.OrderManagement.CrossCuttingConcerns.SharedAppSetting;

namespace PetProject.OrderManagement.CrossCuttingConcerns.Extensions
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
