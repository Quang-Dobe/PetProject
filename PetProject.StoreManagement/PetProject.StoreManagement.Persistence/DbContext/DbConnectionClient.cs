using Microsoft.EntityFrameworkCore;
using PetProject.StoreManagement.Domain.ThirdPartyServices.DbConnectionClient;
using PetProject.StoreManagement.Persistence.SqlServer.Extensions;
using System.Data;

namespace PetProject.StoreManagement.Persistence
{
    public class DbConnectionClient : IDbConnectionClient, IDisposable
    {
        private StoreManagementDbContext _context;

        public DbConnectionClient(StoreManagementDbContext context)
        {
            _context = context;
        }

        public IDbConnection GetDbConnection()
        {
            var connection = _context.GetCurrentConnection();

            if (connection == null)
            {
                _context.Database.OpenConnection();
                return _context.Database.GetDbConnection();
            }

            return connection;
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }
    }
}
