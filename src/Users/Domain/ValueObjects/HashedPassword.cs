using Shared;
using Users.Domain.Exceptions;

namespace Users.Domain.ValueObjects
{
    public class HashedPassword : ValueObject
    {
        public string Hash { get; }

        public HashedPassword(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash)) throw new UserDomainException("Password hash cannot be empty.");

            Hash = hash;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Hash;
        }
    }
}
