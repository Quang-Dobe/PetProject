namespace PetProject.StoreManagement.Domain.Entities.BaseEntity
{
    public interface IHasKey<TId>
    {
        TId Id { get; set; }
    }
}