using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Users.Infrastructure.Persistence
{
    public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseNpgsql("Host=localhost;Port=5432;Database=users;Username=postgres;Password=postgres")
                .Options;

            return new UserDbContext(options, null);
        }
    }
}
