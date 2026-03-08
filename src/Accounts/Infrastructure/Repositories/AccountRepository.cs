using Accounts.Domain.Entities;
using Accounts.Domain.Repositories;
using Accounts.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Accounts.Infrastructure.Repositories
{
    public class AccountRepository(AccountDbContext context) : IAccountRepository
    {
        private readonly AccountDbContext _context = context;

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default)
        { 
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id, ct);
        }

        public async Task<IReadOnlyList<Account>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Accounts.Where(a => a.UserId == userId && a.Status != AccountStatus.Closed).OrderBy(a => a.OpenedAt).ToListAsync(ct);
        }

        public void Add(Account account)
        {
            _context.Accounts.Add(account);
        }

        public void Update(Account account)
        {
            _context.Accounts.Update(account);
        }
    }
}
