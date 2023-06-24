using Microsoft.Extensions.DependencyInjection;
using PetProject.OrderManagement.CrossCuttingConcerns.OS;

namespace PetProject.OrderManagement.CrossCuttingConcerns.Extensions
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
