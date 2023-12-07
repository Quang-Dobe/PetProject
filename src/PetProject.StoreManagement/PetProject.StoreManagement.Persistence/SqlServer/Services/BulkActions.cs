using Microsoft.EntityFrameworkCore;
using PetProject.StoreManagement.Domain.ThirdPartyServices.BulkActions;
using PetProject.StoreManagement.Persistence.SqlServer.BulkAction_V1;

namespace PetProject.StoreManagement.Persistence.SqlServer.Services
{
    public class BulkActions<TDbContext> : IBulkActions where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;

        public BulkActions(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void BulkInsert(IEnumerable<object> data, IEnumerable<string> columnNames)
        {
            _dbContext.BulkInsert(data, columnNames, null);
        }

        public void BulkUpdate(IEnumerable<object> data, IEnumerable<string> columnNames)
        {
            _dbContext.BulkUpdate(data, columnNames, null);
        }

        public void BulkMerge(IEnumerable<object> data, IEnumerable<string> columnNames)
        {
            _dbContext.BulkMerge(data, columnNames, null);
        }

        public void BulkDelete(IEnumerable<object> data, IEnumerable<string> columnNames)
        {
            _dbContext.BulkDelete(data, columnNames, null);
        }
    }
}
