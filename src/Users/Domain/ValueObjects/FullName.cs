using Shared;
using Users.Domain.Exceptions;

namespace Users.Domain.ValueObjects
{
    public class FullName : ValueObject
    {
        public string FirstName { get; }
        public string LastName { get; }

        public FullName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new UserDomainException("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new UserDomainException("Last name cannot be empty.");

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
        }

        public string DisplayName => $"{FirstName} {LastName}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
        }
    }
}
