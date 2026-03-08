using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Accounts.Infrastructure.Persistence
{
    public class AccountDbContextFactory : IDesignTimeDbContextFactory<AccountDbContext>
    {
        public AccountDbContext CreateDbContext(string[] args) 
        {
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseNpgsql("Host=localhost;Port=5432;Database=accounts;Username=postgres;Password=postgres")
                .Options;

            return new AccountDbContext(options, null);
        }
    }
}
