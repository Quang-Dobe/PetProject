using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetProject.StoreManagement.CrossCuttingConcerns.Extensions;
using PetProject.StoreManagement.Domain.Repositories;
using PetProject.StoreManagement.Domain.Entities.BaseEntity;
using PetProject.StoreManagement.Domain.ThirdPartyServices.BulkActions;
using PetProject.StoreManagement.Domain.ThirdPartyServices.DbConnectionClient;
using PetProject.StoreManagement.Persistence.Repositories;
using PetProject.StoreManagement.Persistence.SqlServer.Services;

namespace PetProject.StoreManagement.Persistence.Extensions
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString, string migrationAssembly)
        {
            services.AddScoped<IBaseRepository<BaseEntity<Guid>>, BaseRepository<BaseEntity<Guid>>>();
            services.AddScoped(typeof(IUnitOfWork), services =>
            {
                return services.GetRequiredService<StoreManagementDbContext>();
            });

            services.AddStoreManagementDbContext(connectionString, migrationAssembly);
            services.AddBulkActions();
            services.AddRepositories();

            return services;
        }

        public static IServiceCollection AddStoreManagementDbContext(this IServiceCollection services, string connectionString, string migrationAssembly)
        {
            services.AddDbContext<StoreManagementDbContext>(options => options.UseSqlServer(connectionString, sql =>
            {
                if (!migrationAssembly.IsNullOrEmpty())
                {
                    sql.MigrationsAssembly(migrationAssembly);
                }
            }));

            services.AddScoped<IDbConnectionClient, DbConnectionClient>();

            return services;
        }

        public static IServiceCollection AddBulkActions(this IServiceCollection services)
        {
            services.AddScoped<IBulkActions, BulkActions<StoreManagementDbContext>>();

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
