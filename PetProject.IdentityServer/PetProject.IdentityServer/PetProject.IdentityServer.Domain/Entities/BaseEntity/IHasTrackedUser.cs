namespace PetProject.IdentityServer.Domain.Entities.BaseEntity
{
    public interface IHasTrackedUser<TId>
    {
        /// <summary>
        /// Foreign Key navigate to User
        /// </summary>
        TId? CreatedById { get; set; }
        
        /// <summary>
        /// Foreign Key navigate to User
        /// </summary>
        TId? UpdatedById { get; set; }
    }
}