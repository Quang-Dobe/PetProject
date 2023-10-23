using Nest;
using PetProject.OrderManagement.CrossCuttingConcerns.SharedAppSetting;
using PetProject.OrderManagement.Infrastructure.ElasticsearchServer.DefaultSetting;
using System.Reflection;

namespace PetProject.OrderManagement.Infrastructure.ElasticsearchServer.Extensions
{
    public class ElasticsearchServerSetting
    {
        private readonly AppSettings _appSettings;

        public ElasticsearchServerSetting(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public void SetupElasticsearchServer()
        {
            var baseUrl = _appSettings.ElasticSettings.BaseUrl;
            var userName = _appSettings.ElasticSettings.UserName;
            var password = _appSettings.ElasticSettings.Password;
            var certificate = _appSettings.ElasticSettings.Certificate;
            var index = _appSettings.ElasticSettings.DefaultIndex;

            var settings = new ConnectionSettings(new Uri(baseUrl ?? ""))
                .CertificateFingerprint(certificate).BasicAuthentication(userName, password)
                .EnableApiVersioningHeader().DefaultIndex(index);

            AddDefaultMapping(settings);
            var client = new ElasticClient(settings);
            AddIndex(client, index);
        }

        private void AddDefaultMapping(ConnectionSettings settings)
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
                            defaultMappingMethod.Invoke(null, parameters);
                        }

                    }
                }
            }
        }

        private void AddIndex(IElasticClient client, string indexName)
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
                            defaultIndexMethod.Invoke(null, parameters);
                        }

                    }
                }
            }
        }
    }
}
