using PetProject.StoreManagement.WorkerService;
using PetProject.StoreManagement.WorkerService.Extensions;

var builder = Host.CreateApplicationBuilder(args);
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetService<IConfiguration>();

        var appSettings = new AppSettings(configuration);

        services.AddWorkerService(appSettings);
    })
    .Build();

host.Run();
