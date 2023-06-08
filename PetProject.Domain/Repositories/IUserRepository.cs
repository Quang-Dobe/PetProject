using PetProject.IdentityServer.Domain.Entities;

namespace PetProject.IdentityServer.Domain.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        IQueryable<User> GetUsersWith(UserQueryOptions queryOptions);
    }

    public class UserQueryOptions
    {
        public bool IncludeClaims { get; set; }

        public bool IncludeRoles { get; set; }

        public bool IncludeUserRoles { get; set; }

        public bool AsNoTracking { get; set; }
    }
}