using Microsoft.EntityFrameworkCore;
using Shared;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Users.Infrastructure.Persistence;

namespace Users.Infrastructure.Repositories
{
    public class UserRepository(UserDbContext context) : IUserRepository
    {
        private readonly UserDbContext _context = context;

        public IUnitOfWork UnitOfWork => _context;

        public async Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {             
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(
                u => u.Email.Value.Equals(email, StringComparison.InvariantCultureIgnoreCase), 
                cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AnyAsync(
                u => u.Email.Value.Equals(email, StringComparison.InvariantCultureIgnoreCase),
                cancellationToken);
        }
        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }
    }
}
