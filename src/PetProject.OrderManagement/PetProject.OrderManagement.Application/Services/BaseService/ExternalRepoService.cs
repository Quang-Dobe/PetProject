using PetProject.OrderManagement.Domain.Entities.BaseEntity;
using PetProject.OrderManagement.Domain.Services.BaseService;
using PetProject.OrderManagement.Persistence.Services;

namespace PetProject.OrderManagement.Application.Services
{
    public class ExternalRepoService : BaseService, IExternalRepoService
    {
        public Task GenerateData<T>(T data) where T : BaseEntity<Guid>
        {
            return Task.CompletedTask;
        }

        public Task DeleteData<T>(T data) where T : BaseEntity<Guid>
        {
            return Task.CompletedTask;
        }

        public Task BulkGenerateData<T>(IEnumerable<T> data) where T : BaseEntity<Guid>
        {
            return Task.CompletedTask;
        }

        public Task BulkDeleteData<T>(IEnumerable<T> data) where T : BaseEntity<Guid>
        {
            return Task.CompletedTask;
        }
    }
}
