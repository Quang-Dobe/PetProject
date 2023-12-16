using PetProject.StoreManagement.CrossCuttingConcerns.Extensions;
using PetProject.StoreManagement.Infrastructure.Extensions;
using PetProject.StoreManagement.Persistence.Extensions;

namespace PetProject.StoreManagement.WorkerService.Extensions
{
    public static class WorkerServiceExtensions
    {
        public static IServiceCollection AddWorkerService(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddCrossCuttingConcerns();
            services.AddPersistence(appSettings.ConnectionStrings.StoreManagement, "");
            services.AddInfrastructure("");

            return services;
        }
    }
}
