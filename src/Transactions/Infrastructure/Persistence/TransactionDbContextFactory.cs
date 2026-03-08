using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Transactions.Infrastructure.Persistence
{
    public class AccountDbContextFactory : IDesignTimeDbContextFactory<TransactionDbContext>
    {
        public TransactionDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<TransactionDbContext>()
                .UseNpgsql("Host=localhost;Port=5432;Database=transactions;Username=postgres;Password=postgres")
                .Options;

            return new TransactionDbContext(options, null);
        }
    }
}
