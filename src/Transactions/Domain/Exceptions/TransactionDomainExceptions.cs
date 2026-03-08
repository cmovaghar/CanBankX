using Transactions.Domain.Entities;

namespace Transactions.Domain.Exceptions
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

    public class CurrenciesMissmatchException(string transactionRef, Currency expected, Currency actual) : Exception($"Currency mismatch for transaction [{transactionRef}]. Expected: {expected}, Got: {actual}");

    public class TransactionToAndFromSameException(Guid accountId) : Exception($"Transaction cannot originate and be sent to same account [{accountId}].");

    public class TransactionNotPendingException(Guid transactionId, TransactionStatus currentStatus) : Exception($"Transaction [{transactionId}] is not pending. Current status: {currentStatus}.");
}
