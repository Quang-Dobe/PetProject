using System.Data;

namespace PetProject.StoreManagement.Persistence
{
    public interface IUnitOfWork
    {
        IDisposable BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        void CommitTransaction();

        int SaveChanges();


        Task<IDisposable> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

        Task CommitTransactionAync(CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}