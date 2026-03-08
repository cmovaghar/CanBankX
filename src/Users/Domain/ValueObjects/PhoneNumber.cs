using Shared;
using Users.Domain.Exceptions;

namespace Users.Domain.ValueObjects
{
    public class PhoneNumber : ValueObject
    {
        public string Value { get; }

        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.StartsWith('+') || value.Length < 10) throw new DomainValidationException("PhoneNumber", $"'{value}' is not a valid E.164 phone number.");

            Value = value.Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
