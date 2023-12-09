using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PetProject.StoreManagement.Domain.ThirdPartyServices.ExternalRepoService;
using PetProject.StoreManagement.Infrastructure.ExternalRepoService;
using System.Reflection;

namespace PetProject.StoreManagement.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IExternalRepoService, ExternalRepoService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
