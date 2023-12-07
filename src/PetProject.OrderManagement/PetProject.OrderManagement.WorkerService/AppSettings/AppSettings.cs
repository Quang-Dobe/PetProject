namespace PetProject.OrderManagement.WorkerService.AppSettings;

public class AppSettings
{
    public AppSettings(IConfiguration configuration)
    {
        ElasticSettings = new ElasticSettings(configuration);

        ConnectionStrings = new ConnectionStrings(configuration);
    }

    public ElasticSettings ElasticSettings { get; set; }

    public ConnectionStrings ConnectionStrings { get; set; }
}
