using Microsoft.EntityFrameworkCore;
using PetProject.IdentityServer.CrossCuttingConcerns.OS;
using PetProject.IdentityServer.Domain.Entities;
using PetProject.IdentityServer.Domain.Repositories;

namespace PetProject.IdentityServer.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IdentityDbContext dbContext, IDateTimeProvider dateTimeProvider) : base(dbContext, dateTimeProvider)
        { }

        public IQueryable<User> GetUsersWith(UserQueryOptions? queryOptions)
        {
            var users = GetAll();
            if (queryOptions.IncludeClaims)
            {
                users = users.Include(x => x.UserClaims);
            }
            if (queryOptions.IncludeRoles)
            {
                users = users.Include("UserRoles.Role");
            }
            if (queryOptions.IncludeUserRoles)
            {
                users = users.Include(x => x.UserRoles);
            }
            if (queryOptions.AsNoTracking)
            {
                users = users.AsNoTracking();
            }

            return users;
        }
    }
}
