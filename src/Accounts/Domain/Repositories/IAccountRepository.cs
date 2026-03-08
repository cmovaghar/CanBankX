using Accounts.Domain.Entities;
using Shared;

namespace Accounts.Domain.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<IReadOnlyList<Account>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);

        void Add(Account account);

        void Update(Account account);
    }
}
