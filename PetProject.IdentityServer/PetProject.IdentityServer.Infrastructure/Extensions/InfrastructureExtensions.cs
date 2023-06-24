﻿using Microsoft.Extensions.DependencyInjection;
using PetProject.IdentityServer.CrossCuttingConcerns.HtmlGenerator;
using PetProject.IdentityServer.Infrastructure.HtmlGeneratorServices;
using RazorLight;

namespace PetProject.IdentityServer.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddHtmlGenerator();

            return services;
        }

        public static IServiceCollection AddHtmlGenerator(this IServiceCollection services)
        {
            var engine = new RazorLightEngineBuilder()
                  .UseFileSystemProject(Environment.CurrentDirectory)
                  .UseMemoryCachingProvider()
                  .Build();

            services.AddSingleton<IRazorLightEngine>(engine);
            services.AddSingleton<IHtmlGenerator, HtmlGenerator>();

            return services;
        }
    }
}