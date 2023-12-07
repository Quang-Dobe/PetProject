using PetProject.OrderManagement.Domain.Entities.BaseEntity;

namespace PetProject.OrderManagement.Domain.Services.BaseService
{
    public interface IExternalRepoService
    {
        Task GenerateData<T>(T data) where T : BaseEntity<Guid>;
        Task DeleteData<T>(T data) where T : BaseEntity<Guid>;
        Task BulkGenerateData<T>(IEnumerable<T> data) where T : BaseEntity<Guid>;
        Task BulkDeleteData<T>(IEnumerable<T> data) where T : BaseEntity<Guid>;
    }
}
