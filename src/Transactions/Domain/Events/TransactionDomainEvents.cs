using Shared;
using Transactions.Domain.Entities;

namespace Transactions.Domain.Events
{
    public record TransactionCreatedDomainEvent(Transaction Transaction) : IDomainEvent;
    public record TransactionCompletedDomainEvent(Transaction Transaction) : IDomainEvent;
    public record TransactionFailedDomainEvent(Transaction Transaction) : IDomainEvent;
}
