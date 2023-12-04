using Microsoft.Data.SqlClient;
using PetProject.StoreManagement.Domain.ThirdPartyServices.DbConnectionClient;
using System.Data;

namespace PetProject.StoreManagement.Persistence
{
    public class DbConnectionClient : IDbConnectionClient, IDisposable
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public DbConnectionClient(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetDbConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }

            return _connection;
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Dispose();
            }
        }
    }
}
