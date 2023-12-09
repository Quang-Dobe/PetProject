using Nest;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PetProject.OrderManagement.Infrastructure.ElasticsearchServer.DefaultSetting;
using PetProject.OrderManagement.CrossCuttingConcerns.SharedAppSetting;
using PetProject.OrderManagement.Domain.Services.BaseService;

namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.Extensions
{
    public static class ElasticsearchServerExtension
    {
        public static IServiceCollection AddElasticsearchServer(this IServiceCollection services, string baseUrl, string userName, string password, string certificate, string defaultIndex)
        {
            var config = new ElasticSearchConfiguration();

            var settings = new ConnectionSettings(new Uri(config.BaseUrl ?? baseUrl))
                .CertificateFingerprint(config.Certificate ?? certificate).BasicAuthentication(config.UserName ?? userName, config.Password ?? password)
                .EnableApiVersioningHeader().DefaultIndex(config.DefaultIndex ?? defaultIndex);

            AddDefaultMapping(settings);
            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);
            AddIndex(client, config.DefaultIndex ?? defaultIndex);

            return services;
        }

        private static void AddDefaultMapping(ConnectionSettings settings)
        {
            foreach (var exportedType in Assembly.GetExecutingAssembly().GetExportedTypes())
            {
                if (exportedType.IsClass && !exportedType.IsAbstract)
                {
                    var interfaceTypes = exportedType.GetInterfaces();
                    if (interfaceTypes.Length == 1 && interfaceTypes.Contains(typeof(IBaseDefaultSetting)))
                    {
                        // Add default mapping
                        var defaultMappingMethod = exportedType.GetMethod("SetDefaultMapping");
                        if (defaultMappingMethod != null && defaultMappingMethod.ReturnType == typeof(void))
                        {
                            object[] parameters = { settings };
                            defaultMappingMethod.Invoke(Activator.CreateInstance(exportedType), parameters);
                        }

                    }
                }
            }
        }

        private static void AddIndex(IElasticClient client, string indexName)
        {
            foreach (var exportedType in Assembly.GetExecutingAssembly().GetExportedTypes())
            {
                if (exportedType.IsClass && !exportedType.IsAbstract)
                {
                    var interfaceTypes = exportedType.GetInterfaces();
                    if (interfaceTypes.Length == 1 && interfaceTypes.Contains(typeof(IBaseDefaultSetting)))
                    {
                        // Add index
                        var defaultIndexMethod = exportedType.GetMethod("SetIndex");
                        if (defaultIndexMethod != null && defaultIndexMethod.ReturnType == typeof(void))
                        {
                            object[] parameters = { client, indexName };
                            defaultIndexMethod.Invoke(Activator.CreateInstance(exportedType), parameters);
                        }

                    }
                }
            }
        }
    }
}
