using PetProject.StoreManagement.Domain.Entities.BaseEntity;
using PetProject.StoreManagement.Domain.ThirdPartyServices.ExternalRepoService;

namespace PetProject.StoreManagement.Infrastructure.ExternalRepoService
{
    public class ExternalRepoService : IExternalRepoService
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
