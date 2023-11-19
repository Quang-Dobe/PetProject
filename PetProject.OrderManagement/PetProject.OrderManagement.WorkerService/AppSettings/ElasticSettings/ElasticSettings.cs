
namespace PetProject.OrderManagement.WorkerService.AppSettings;

public class ElasticSettings
{
    public ElasticSettings(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private readonly IConfiguration _configuration;

    public string BaseUrl => _configuration["ElasticSettings:BaseUrl"] ?? "";

    public string UserName => _configuration["ElasticSettings:UserName"] ?? "";

    public string Password => _configuration["ElasticSettings:Password"] ?? "";

    public string Certificate => _configuration["ElasticSettings:Certificate"] ?? "";

    public string DefaultIndex => _configuration["ElasticSettings:DefaultIndex"] ?? "";
}
