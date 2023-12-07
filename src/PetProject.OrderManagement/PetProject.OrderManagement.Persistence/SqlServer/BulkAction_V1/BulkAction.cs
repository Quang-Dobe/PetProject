using Microsoft.EntityFrameworkCore;
using PetProject.OrderManagement.Persistence.SqlServer.BulkAction_V1.BulkDelete;
using PetProject.OrderManagement.Persistence.SqlServer.BulkAction_V1.BulkInsert;
using PetProject.OrderManagement.Persistence.SqlServer.BulkAction_V1.BulkMerge;
using PetProject.OrderManagement.Persistence.SqlServer.BulkAction_V1.BulkUpdate;
using PetProject.OrderManagement.Persistence.SqlServer.Extensions;

namespace PetProject.OrderManagement.Persistence.SqlServer.BulkAction_V1
{
    public static class BulkAction
    {
        public static void BulkInsert<T, TDbContext>(this TDbContext dbContext, IEnumerable<T> data, IEnumerable<string> columnNames, Action<BulkOptions>? action) where TDbContext : DbContext
        {
            var connection = dbContext.GetCurrentConnection();
            var transaction = dbContext.GetCurrentTransaction();
            var tableName = dbContext.GetDbTableName(typeof(T));
            var primaryKeys = dbContext.GetPrimaryKeys(typeof(T));
            var dbColumnMappings = dbContext.GetDbColumnMappings(typeof(T));

            new BulkInsertBuilder<T>(connection, transaction)
                .WithTable(tableName)
                .WithTablePrefix("dbo")
                .WithData(data)
                .WithIdColumns(primaryKeys)
                .WithColumnNames(columnNames)
                .WithDbColumnMappings(dbColumnMappings)
                .WithConfigureBulkOptions(action)
                .Excute();
        }

        public static void BulkUpdate<T, TDbContext>(this TDbContext dbContext, IEnumerable<T> data, IEnumerable<string> columnNames, Action<BulkOptions>? action) where TDbContext : DbContext
        {
            var connection = dbContext.GetCurrentConnection();
            var transaction = dbContext.GetCurrentTransaction();
            var tableName = dbContext.GetDbTableName(typeof(T));
            var primaryKeys = dbContext.GetPrimaryKeys(typeof(T));
            var dbColumnMappings = dbContext.GetDbColumnMappings(typeof(T));

            new BulkUpdateBuilder<T>(connection, transaction)
                .WithTable(tableName)
                .WithTablePrefix("dbo")
                .WithData(data)
                .WithIdColumns(primaryKeys)
                .WithColumnNames(columnNames)
                .WithDbColumnMappings(dbColumnMappings)
                .WithConfigureBulkOptions(action)
                .Excute();
        }

        public static void BulkDelete<T, TDbContext>(this TDbContext dbContext, IEnumerable<T> data, IEnumerable<string> columnNames, Action<BulkOptions>? action) where TDbContext : DbContext
        {
            var connection = dbContext.GetCurrentConnection();
            var transaction = dbContext.GetCurrentTransaction();
            var tableName = dbContext.GetDbTableName(typeof(T));
            var primaryKeys = dbContext.GetPrimaryKeys(typeof(T));
            var dbColumnMappings = dbContext.GetDbColumnMappings(typeof(T));

            new BulkDeleteBuilder<T>(connection, transaction)
                .WithTable(tableName)
                .WithTablePrefix("dbo")
                .WithData(data)
                .WithIdColumns(primaryKeys)
                .WithColumnNames(columnNames)
                .WithDbColumnMappings(dbColumnMappings)
                .WithConfigureBulkOptions(action)
                .Excute();
        }

        public static void BulkMerge<T, TDbContext>(this TDbContext dbContext, IEnumerable<T> data, IEnumerable<string> columnNames, Action<BulkOptions>? action) where TDbContext : DbContext
        {
            var connection = dbContext.GetCurrentConnection();
            var transaction = dbContext.GetCurrentTransaction();
            var tableName = dbContext.GetDbTableName(typeof(T));
            var primaryKeys = dbContext.GetPrimaryKeys(typeof(T));
            var dbColumnMappings = dbContext.GetDbColumnMappings(typeof(T));

            new BulkMergeBuilder<T>(connection, transaction)
                .WithTable(tableName)
                .WithTablePrefix("dbo")
                .WithData(data)
                .WithIdColumns(primaryKeys)
                .WithColumnNames(columnNames)
                .WithDbColumnMappings(dbColumnMappings)
                .WithConfigureBulkOptions(action)
                .Excute();
        }
    }
}
