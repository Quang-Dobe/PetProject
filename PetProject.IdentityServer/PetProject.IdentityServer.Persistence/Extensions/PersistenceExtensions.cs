using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetProject.IdentityServer.CrossCuttingConcerns.Extensions;
using PetProject.IdentityServer.Domain.Entities;
using PetProject.IdentityServer.Domain.Entities.BaseEntity;
using PetProject.IdentityServer.Domain.Repositories;
using PetProject.IdentityServer.Persistence.MyIdentity;
using PetProject.IdentityServer.Persistence.MyIdentity.MyManager;
using PetProject.IdentityServer.Persistence.Repositories;

namespace PetProject.IdentityServer.Persistence.Extensions
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString, string migrationAssembly)
        {
            services.AddScoped<IBaseRepository<BaseEntity<Guid>>, BaseRepository<BaseEntity<Guid>>>();
            services.AddScoped(typeof(IUnitOfWork), services =>
            {
                return services.GetRequiredService<IdentityDbContext>();
            });

            services.AddIdentityDbContext(connectionString, migrationAssembly);
            services.AddRepositories();

            return services;
        }

        public static IServiceCollection AddIdentityDbContext(this IServiceCollection services, string connectionString, string migrationAssembly)
        {
            services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(connectionString, sql =>
            {
                if (!migrationAssembly.IsNullOrEmpty())
                {
                    sql.MigrationsAssembly(migrationAssembly);
                }
            }));

            services.AddIdentityCore<User>();
            services.AddTransient<IPasswordHasher<User>, PasswordHasher>();
            services.AddTransient<IUserStore<User>, UserStore>();
            services.AddTransient<MyUserManager, MyUserManager>();
            services.Configure<PasswordHasherOptions>(option =>
            {
                option.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2;
            });

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            foreach (var exportedType in Assembly.GetExecutingAssembly().GetExportedTypes())
            {
                if (exportedType.IsClass && !exportedType.IsAbstract)
                {
                    var interfaceTypes = exportedType.GetInterfaces();
                    if (interfaceTypes.Length > 1 && interfaceTypes.First().Name.StartsWith("IBaseRepository"))
                    {
                        services.AddScoped(interfaceTypes.ElementAtOrDefault(1), exportedType);
                    }
                }
            }

            return services;
        }
    }
}
