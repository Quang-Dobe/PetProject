using Microsoft.Extensions.DependencyInjection;
using PetProject.IdentityServer.CrossCuttingConcerns.OS;
using PetProject.IdentityServer.CrossCuttingConcerns.SharedAppSetting;

namespace PetProject.IdentityServer.CrossCuttingConcerns.Extensions
{
    public static class CrossCuttingConcernsExtensions
    {
        public static IServiceCollection AddCrossCuttingConcerns(this IServiceCollection services)
        {
            services.AddSharedAppSetting();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            return services;
        }

        public static IServiceCollection AddSharedAppSetting(this IServiceCollection services)
        {
            services.AddSingleton<Jwt>();
            services.AddSingleton<AccessTokenLifetime>();
            services.AddSingleton<RefreshTokenLifetime>();
            services.AddSingleton<ConnectionStrings>();
            services.AddSingleton<EmailSender>();
            services.AddSingleton<AppSettings>();

            return services;
        }
    }
}
