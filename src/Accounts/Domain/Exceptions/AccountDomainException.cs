using Accounts.Domain.Entities;
using Accounts.Domain.ValueObjects;

namespace Accounts.Domain.Exceptions
{
    // 400, can expose to user
    public class DomainValidationException(string field, string message) : Exception(message)
    {
        public string Field { get; } = field;
    }

    // 500, do not expose to user
    public class ValueEmptyException(string field, string message) : Exception(message)
    {
        public string Field { get; } = field;
    }

    public class CurrenciesMissmatchException(Currency expected, Currency actual) : Exception($"Currency mismatch. Expected: {expected}, Got: {actual}");

    public class InsufficientFundsException(Guid accountId, Money balance, Money requested) : Exception($"Account {accountId} has insufficient funds. Balance: {balance}, Requested: {requested}");

    public class AccountNotActiveException(Guid accountId, AccountStatus status) : Exception($"Account {accountId} is not active. Current status: {status}");

    public class AccountNotEmptyException(Guid accountId, Money balance) : Exception($"Account {accountId} cannot be closed. Remaining balance: {balance}");

    public class CurrencyMismatchException(Currency expected, Currency actual) : Exception($"Currency mismatch. Expected: {expected}, Got: {actual}");

    public class AccountNotFoundException(Guid accountId) : Exception($"Account {accountId} was not found.");

    public class AccountOwnershipException(Guid accountId, Guid requestingUserId) : Exception($"User {requestingUserId} does not own account {accountId}.");
}
