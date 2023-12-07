using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using PetProject.OrderManagement.CrossCuttingConcerns.OS;
using PetProject.OrderManagement.Domain.Entities.BaseEntity;
using System.Data;

namespace PetProject.OrderManagement.Persistence
{
    public class OrderManagementDbContext : DbContext, IUnitOfWork
    {
        private IDbContextTransaction _transaction;

        private readonly IDateTimeProvider _dateTimeProvider;

        public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options, IDateTimeProvider dateTimeProvider) : base(options)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        }

        #region Implement IUnitOfWork

        public IDisposable BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _transaction = Database.BeginTransaction(isolationLevel);

            return _transaction;
        }

        public void CommitTransaction()
        {
            _transaction.Commit();
        }

        public override int SaveChanges()
        {
            TrackingInformation();
            return base.SaveChanges();
        }

        public async Task<IDisposable> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
        {
            _transaction = await Database.BeginTransactionAsync(isolationLevel, cancellationToken);

            return _transaction;
        }

        public async Task CommitTransactionAync(CancellationToken cancellationToken = default)
        {
            await _transaction.CommitAsync(cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            TrackingInformation();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void TrackingInformation()
        {
            var abstractEntites = ChangeTracker.Entries<AbstractEntity<Guid>>();
            var aggregateEntities = ChangeTracker.Entries<AggregateEntity<Guid>>();
            AddingDateTimeInformation(abstractEntites);
            AddingDateTimeInformation(aggregateEntities);
            AddSyncInformation(abstractEntites);
            AddSyncInformation(aggregateEntities);
        }

        private void AddingDateTimeInformation<TEntityType>(IEnumerable<EntityEntry<TEntityType>> entityEntries) where TEntityType : BaseEntity<Guid>
        {
            foreach (var entity in entityEntries.Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted))
            {
                if (entity.State == EntityState.Added)
                {
                    entity.Entity.CreatedDate = _dateTimeProvider.Now;
                    entity.Entity.CreatedDateTimeOffset = _dateTimeProvider.OffsetNow;
                }
                else if (entity.State == EntityState.Modified)
                {
                    entity.Entity.UpdatedDate = _dateTimeProvider.Now;
                    entity.Entity.UpdatedDateTimeOffset = _dateTimeProvider.OffsetNow;
                }
                else
                {
                    entity.Entity.DeletedDate = _dateTimeProvider.Now;
                    entity.Entity.DeletedDateTimeOffset = _dateTimeProvider.OffsetNow;
                }
            }
        }

        private void AddSyncInformation<TEntityType>(IEnumerable<EntityEntry<TEntityType>> entityEntries) where TEntityType : BaseEntity<Guid>
        {
            foreach (var entity in entityEntries.Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted))
            {
                entity.Entity.IsSync = false;
            }
        }

        #endregion
    }
}
