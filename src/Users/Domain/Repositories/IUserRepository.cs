using Shared;
using Users.Domain.Entities;

namespace Users.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        void Add(User user);
        void Update(User user);
    }
}
