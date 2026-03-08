using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared;
using Transactions.Domain.Entities;
using Transactions.Domain.ValueObjects;

namespace Transactions.Infrastructure.Persistence
{
    public class TransactionDbContext(DbContextOptions<TransactionDbContext> options, IMediator? mediator) : DbContext(options), IUnitOfWork
    {
        private readonly IMediator? _mediator = mediator;

        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransactionDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchDomainEventsAsync(cancellationToken);
            await SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
        {
            var domainEntities = ChangeTracker
                .Entries<Entity>()
                .Where(e => e.Entity.DomainEvents?.Count > 0)
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(e => e.Entity.DomainEvents!)
                .ToList();

            domainEntities.ForEach(e => e.Entity.ClearDomainEvents());

            if (_mediator != null)
            {
                foreach (var domainEvent in domainEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }
            }
        }
    }
}
