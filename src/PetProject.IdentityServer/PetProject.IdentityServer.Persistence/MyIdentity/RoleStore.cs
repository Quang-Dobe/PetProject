using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetProject.IdentityServer.CrossCuttingConcerns.Extensions;
using PetProject.IdentityServer.Domain.Entities;
using PetProject.IdentityServer.Domain.Repositories;

namespace PetProject.Persistence.MyIdentity
{
    public class RoleStore : IRoleStore<Role>
    {
        private readonly IRoleRepository _roleRepository;

        public RoleStore(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            await _roleRepository.AddAsync(role, cancellationToken);

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            _roleRepository.Delete(role);

            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<Role?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            Guid id = Guid.NewGuid();
            if (Guid.TryParse(roleId, out id))
            {
                var role = await _roleRepository.GetAll().Where(x => x.Id == id).FirstOrDefaultAsync();

                return role;
            }

            throw new InvalidDataException("Not exist role");
        }

        public async Task<Role?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetAll().Where(x => x.NormalizedName == normalizedRoleName).FirstOrDefaultAsync();

            return role;
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string?> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task<string?> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(Role role, string? normalizedName, CancellationToken cancellationToken)
        {
            if (!normalizedName.IsNullOrEmpty())
            {
                role.NormalizedName = normalizedName;
            }

            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(Role role, string? roleName, CancellationToken cancellationToken)
        {
            if (!roleName.IsNullOrEmpty())
            {
                role.Name = roleName;
            }

            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            var specificRole = await _roleRepository.GetAll()
                .Where(x => x.Id == role.Id 
                    || x.Name == role.Name 
                    || x.NormalizedName == role.NormalizedName)
                .FirstOrDefaultAsync();
            
            if (specificRole != null)
            {
                _roleRepository.Update(role);
            }

            return IdentityResult.Success;
        }

        public void Dispose()
        { }
    }
}
