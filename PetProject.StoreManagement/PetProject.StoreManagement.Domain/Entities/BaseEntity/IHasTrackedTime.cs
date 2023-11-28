namespace PetProject.StoreManagement.Domain.Entities.BaseEntity
{
    public interface IHasTrackedTime
    {
        [Obsolete("Will be migrated to CreatedDateTimeOffset")]
        DateTime? CreatedDate { get; set; }

        [Obsolete("Will be migrated to UpdatedDateTimeOffset")]
        DateTime? UpdatedDate { get; set; }

        DateTime? DeletedDate { get; set; }

        DateTimeOffset? CreatedDateTimeOffset { get; set; }

        DateTimeOffset? UpdatedDateTimeOffset { get; set; }

        DateTimeOffset? DeletedDateTimeOffset { get; set; }
    }
}