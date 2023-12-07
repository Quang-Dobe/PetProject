namespace PetProject.OrderManagement.Domain.Entities.BaseEntity
{
    public abstract class AbstractEntity<TId> : BaseEntity<TId>, IHasTrackedUser<TId>
    {
        public virtual TId? CreatedById { get; set; }

        public virtual TId? UpdatedById { get; set; }
    }
}