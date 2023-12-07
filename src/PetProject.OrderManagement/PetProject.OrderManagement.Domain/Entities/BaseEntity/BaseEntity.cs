using System.ComponentModel.DataAnnotations;

namespace PetProject.OrderManagement.Domain.Entities.BaseEntity
{
    public abstract class BaseEntity<TId> : IHasKey<TId>, IHasTracked, IHasTrackedTime
    {
        public virtual TId Id { get; set; }

        [Timestamp]
        public virtual byte[] Version { get; set; }

        public virtual bool IsSync { get; set; }

        public virtual bool RowDeleted { get; set; }

        public virtual DateTime? CreatedDate { get; set; }

        public virtual DateTime? UpdatedDate { get; set; }

        public virtual DateTime? DeletedDate { get; set; }

        public virtual DateTimeOffset? CreatedDateTimeOffset { get; set; }

        public virtual DateTimeOffset? UpdatedDateTimeOffset { get; set; }

        public virtual DateTimeOffset? DeletedDateTimeOffset { get; set; }
    }
}