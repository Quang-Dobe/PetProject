using Microsoft.EntityFrameworkCore;
using PetProject.OrderManagement.Infrastructure.SqlServer.BulkAction_V1.BulkDelete;
using PetProject.OrderManagement.Infrastructure.SqlServer.BulkAction_V1.BulkInsert;
using PetProject.OrderManagement.Infrastructure.SqlServer.BulkAction_V1.BulkMerge;
using PetProject.OrderManagement.Infrastructure.SqlServer.BulkAction_V1.BulkUpdate;
using PetProject.OrderManagement.Infrastructure.SqlServer.Extensions;

namespace PetProject.OrderManagement.Infrastructure.SqlServer.BulkAction_V1
{
    public static class BulkAction
    {
        public static void BulkInsert<T>(this DbContext dbContext, IEnumerable<T> data, IEnumerable<string> columnNames, Action<BulkOptions>? action)
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

        public static void BulkUpdate<T>(this DbContext dbContext, IEnumerable<T> data, IEnumerable<string> columnNames, Action<BulkOptions>? action)
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

        public static void BulkDelete<T>(this DbContext dbContext, IEnumerable<T> data, IEnumerable<string> columnNames, Action<BulkOptions>? action)
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

        public static void BulkMerge<T>(this DbContext dbContext, IEnumerable<T> data, IEnumerable<string> columnNames, Action<BulkOptions>? action)
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
