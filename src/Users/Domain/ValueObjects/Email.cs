using Shared;
using System.Net;
using Users.Domain.Exceptions;

namespace Users.Domain.ValueObjects
{
    public class Email : ValueObject
    {
        public string Value { get; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains('@')) throw new DomainValidationException("Email", $"'{value}' is not a valid email address.");

            Value = value.Trim().ToLowerInvariant();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
