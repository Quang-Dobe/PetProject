using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace PetProject.OrderManagement.Persistence.SqlServer.Extensions
{
    public static class DbContextExtensions
    {
        public static string? GetDbTableName(this DbContext dbContext, Type typeOfEntity)
        {
            return dbContext.Model.FindEntityType(typeOfEntity)?.GetTableName();
        }

        public static IEnumerable<string>? GetPrimaryKeys(this DbContext dbContext, Type typeOfEntity)
        {
            return dbContext.Model.FindEntityType(typeOfEntity).FindPrimaryKey().Properties.Select(x => x.Name);
        }

        public static IDictionary<string, string> GetDbColumnMappings(this DbContext dbContext, Type typeOfEntity)
        {
            return dbContext.Model.FindEntityType(typeOfEntity).GetProperties().ToDictionary(x => x.GetDefaultColumnName(), x => x.Name); ;
        }

        public static IDbConnection? GetCurrentConnection(this DbContext dbContext)
        {
            var connection = dbContext.Database.GetDbConnection();
            return connection;
        }

        public static IDbTransaction? GetCurrentTransaction(this DbContext dbContext)
        {
            var transaction = dbContext.Database.CurrentTransaction;

            return transaction == null ? null : transaction.GetDbTransaction();
        }
    }
}
