﻿using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetProject.OrderManagement.CrossCuttingConcerns.Extensions;
using PetProject.OrderManagement.Domain.Entities.BaseEntity;
using PetProject.OrderManagement.Domain.Repositories;
using PetProject.OrderManagement.Persistence.Repositories;

namespace PetProject.OrderManagement.Persistence.Extensions
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString, string migrationAssembly)
        {
            services.AddScoped<IBaseRepository<BaseEntity<Guid>>, BaseRepository<BaseEntity<Guid>>>();
            services.AddScoped(typeof(IUnitOfWork), services =>
            {
                return services.GetRequiredService<OrderManagementDbContext>();
            });

            services = AddOrderManagementDbContext(services, connectionString, migrationAssembly);
            services = AddRepositories(services);

            return services;
        }

        public static IServiceCollection AddOrderManagementDbContext(IServiceCollection services, string connectionString, string migrationAssembly)
        {
            services.AddDbContext<OrderManagementDbContext>(options => options.UseSqlServer(connectionString, sql =>
            {
                if (!migrationAssembly.IsNullOrEmpty())
                {
                    sql.MigrationsAssembly(migrationAssembly);
                }
            }));

            return services;
        }

        public static IServiceCollection AddRepositories(IServiceCollection services)
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
