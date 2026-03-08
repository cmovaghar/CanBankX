using Shared;
using Transactions.Domain.Entities;

namespace Transactions.Domain.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<Transaction?> GetByIdempotencyKeyAsync(Guid idempotenceKey, CancellationToken ct = default);

        Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<IReadOnlyList<Transaction>> GetBySourceIdAsync(Guid id, CancellationToken ct = default);

        void Add(Transaction transaction);

        void Update(Transaction transaction);
    }
}
