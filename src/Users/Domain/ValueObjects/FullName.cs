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
            if (string.IsNullOrWhiteSpace(firstName)) throw new DomainValidationException("FirstName", $"'{firstName}' is not a valid first name.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new DomainValidationException("LastName", $"'{lastName}' is not a valid first name.");

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
