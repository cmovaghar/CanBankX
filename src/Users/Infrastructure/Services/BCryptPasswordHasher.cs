using Users.Domain.Services;

namespace Users.Infrastructure.Services
{
    public class BcryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string plainTextPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainTextPassword);
        }

        public bool Verify(string plainTextPassword, string hash)
        { 
            return BCrypt.Net.BCrypt.Verify(plainTextPassword, hash);
        }
    }
}
