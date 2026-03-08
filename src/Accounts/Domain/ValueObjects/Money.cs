using Accounts.Domain.Entities;
using Accounts.Domain.Exceptions;
using Shared;

namespace Accounts.Domain.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; }
        public Currency Currency { get; }

        public Money(decimal amount, Currency currency)
        {
            if (amount < 0) throw new DomainValidationException(nameof(amount), "Money amount cannot be negative.");

            Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
            Currency = currency;
        }

        public static Money Zero(Currency currency) => new(0m, currency);

        public static Money Of(decimal amount, Currency currency) => new(amount, currency);

        public Money Add(Money other)
        {
            EnsureSameCurrency(other);
            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            EnsureSameCurrency(other);
            return new Money(Amount - other.Amount, Currency);
        }

        public bool IsGreaterThan(Money other)
        {
            EnsureSameCurrency(other);
            return Amount > other.Amount;
        }

        public bool IsGreaterThanOrEqual(Money other)
        {
            EnsureSameCurrency(other);
            return Amount >= other.Amount;
        }

        private void EnsureSameCurrency(Money other)
        {
            if (Currency != other.Currency) throw new CurrenciesMissmatchException(Currency, other.Currency);
        }

        public override string ToString() => $"{Amount:F2} {Currency}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}
