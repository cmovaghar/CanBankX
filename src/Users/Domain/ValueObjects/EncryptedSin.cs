using Shared;
using Users.Domain.Exceptions;

namespace Users.Domain.ValueObjects
{
    public class EncryptedSin : ValueObject
    {
        public string Encrypted { get; }

        public EncryptedSin(string encrypted)
        {
            if (string.IsNullOrWhiteSpace(encrypted)) throw new ValueEmptyException("EncryptedSin", "Encrypted SIN cannot be empty.");

            Encrypted = encrypted;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Encrypted;
        }
    }
}
