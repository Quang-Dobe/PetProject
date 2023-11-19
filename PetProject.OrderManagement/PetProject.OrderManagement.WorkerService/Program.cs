using PetProject.OrderManagement.WorkerService.AppSettings;
using PetProject.OrderManagement.WorkerService.Extensions;

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
