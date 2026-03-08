using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Domain.Services;

namespace Users.Infrastructure.Persistence
{
    public static class UserDbContextSeed
    {
        public static async Task SeedAsync(UserDbContext db, IPasswordHasher hasher)
        {
            if (await db.Users.AnyAsync(u => u.Email.Value == "admin@admin.com")) return;

            var admin = User.Register(
                new("admin@admin.com"),
                new(hasher.Hash("admin")),
                new("Admin", "Admin"),
                DateOnly.Parse("2000-01-01"),
                new("123", "Montreal", "QC", "A1A1A1"),
                new("123456789"),
                new("+15145145144"));

            admin.ConfirmEmail();
            admin.StartKyc();
            admin.ApproveKyc();

            db.Users.Add(admin);

            if (await db.Users.AnyAsync(u => u.Email.Value == "test@test.com")) return;

            var testUser = User.Register(
                new("test@test.com"),
                new(hasher.Hash("test")),
                new("Test", "Test"),
                DateOnly.Parse("2000-01-01"),
                new("123", "Montreal", "QC", "A1A1A1"),
                new("123456789"),
                new("+15145145144"));

            db.Users.Add(testUser);

            await db.SaveChangesAsync();
        }
    }
}
