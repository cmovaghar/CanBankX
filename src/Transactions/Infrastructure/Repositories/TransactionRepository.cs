using Microsoft.EntityFrameworkCore;
using Shared;
using Transactions.Domain.Entities;
using Transactions.Domain.Repositories;
using Transactions.Infrastructure.Persistence;

namespace Transactions.Infrastructure.Repositories
{
    public class TransactionRepository(TransactionDbContext context) : ITransactionRepository
    {
        private readonly TransactionDbContext _context = context;

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Transaction?> GetByIdempotencyKeyAsync(Guid idempotencyKey, CancellationToken ct = default)
        {
            return await _context.Transactions.FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey, ct);
        }

        public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        public async Task<IReadOnlyList<Transaction>> GetBySourceIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Transactions.Where(t => t.FromUser == id).OrderByDescending(t => t.CreatedAt).ToListAsync(ct);
        }

        public void Add(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
        }

        public void Update(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
        }
    }
}
