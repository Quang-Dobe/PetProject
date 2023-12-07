using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PetProject.IdentityServer.Domain.Services;
using PetProject.IdentityServer.Application.Services;

namespace PetProject.IdentityServer.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IBaseService, BaseService>();
            services = AddServices(services);

            return services;
        }

        public static IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            foreach (var exportedType in Assembly.GetExecutingAssembly().GetExportedTypes())
            {
                if (exportedType.IsClass && !exportedType.IsAbstract)
                {
                    var interfaceTypes = exportedType.GetInterfaces();
                    if (interfaceTypes.Length > 1 && interfaceTypes.FirstOrDefault().Equals(typeof(IBaseService)))
                    {
                        services.AddScoped(interfaceTypes.ElementAtOrDefault(1), exportedType);
                    }
                }
            }

            return services;
        }
    }
}
