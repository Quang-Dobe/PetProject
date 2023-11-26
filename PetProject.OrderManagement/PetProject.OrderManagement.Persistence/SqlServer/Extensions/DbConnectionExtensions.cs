using System.Data;

namespace PetProject.OrderManagement.Persistence.SqlServer.Extensions
{
    public static class DbConnectionExtensions
    {
        public static void EnsureOpen(this IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public static void EnsureClosed(this IDbConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public static IDbCommand CreateTextCommand(this IDbConnection connection, IDbTransaction transaction, string commandText) 
        { 
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = commandText;

            return command;
        }
    }
}
