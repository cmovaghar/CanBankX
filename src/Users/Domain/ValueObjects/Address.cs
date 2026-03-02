using Shared;
using Users.Domain.Exceptions;

namespace Users.Domain.ValueObjects
{
    public class Address : ValueObject
    {
        public string Street { get; }
        public string City { get; }
        public string Province { get; }
        public string PostalCode { get; }
        public string Country { get; }

        private static readonly HashSet<string> CanadianProvinces =
        [
            "AB", "BC", "MB", "NB", "NL", "NS", "NT", "NU", "ON", "PE", "QC", "SK", "YT"
        ];

        public Address(string street, string city, string province, string postalCode, string country = "CA")
        {
            if (string.IsNullOrWhiteSpace(street)) throw new UserDomainException("Street cannot be empty.");
            if (string.IsNullOrWhiteSpace(city)) throw new UserDomainException("City cannot be empty.");

            var prov = province.Trim().ToUpperInvariant();
            if (country == "CA" && !CanadianProvinces.Contains(prov)) throw new UserDomainException($"'{province}' is not a valid Canadian province/territory code.");

            var pc = postalCode.Trim().ToUpperInvariant().Replace(" ", "");
            if (country == "CA" && pc.Length != 6) throw new UserDomainException($"'{postalCode}' is not a valid Canadian postal code.");

            Street = street.Trim();
            City = city.Trim();
            Province = prov;
            PostalCode = pc;
            Country = country;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return Province;
            yield return PostalCode;
            yield return Country;
        }
    }
}
