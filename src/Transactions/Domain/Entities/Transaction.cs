using Shared;
using Transactions.Domain.Events;
using Transactions.Domain.Exceptions;
using Transactions.Domain.ValueObjects;

namespace Transactions.Domain.Entities
{
    public sealed class Transaction : Entity, IAggregateRoot
    {
        public Guid IdempotencyKey { get; private set; }
        public Guid FromUser { get; private set; }
        public Guid FromAccount { get; private set; }
        public TransactionDestinationType DestinationType { get; private set; }
        public string Destination { get; private set; } = null!;
        public Money Amount { get; private set; } = null!;
        public TransactionStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public string? FailureReason { get; private set; }

        private Transaction() { }

        private Transaction(Guid idempotencyKey, Guid fromUser, Guid fromAccount, TransactionDestinationType destinationType, string destination, Money amount)
        {
            IdempotencyKey = idempotencyKey;
            Id = Guid.NewGuid();
            FromUser = fromUser;
            FromAccount = fromAccount;
            DestinationType = destinationType;
            Destination = destination;
            Amount = amount;
            Status = TransactionStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public static Transaction Initiate(Guid idempotencyKey, Guid fromUser, Guid fromAccount, TransactionDestinationType destinationType, string destination, Money amount)
        {
            if (idempotencyKey == Guid.Empty) throw new ValueEmptyException(nameof(idempotencyKey), "Idempotency key cannot be empty.");
            if (fromUser == Guid.Empty) throw new ValueEmptyException(nameof(fromUser), "FromUser cannot be empty.");
            if (fromAccount == Guid.Empty) throw new ValueEmptyException(nameof(fromAccount), "FromAccount cannot be empty.");

            if (destinationType == TransactionDestinationType.Account)
            {
                if (!Guid.TryParse(destination, out var destinationAccountId))
                    throw new ValueEmptyException(nameof(destination), "Destination must be a valid account ID.");

                if (destinationAccountId == fromAccount) throw new TransactionToAndFromSameException(destinationAccountId);
            }

            Transaction transaction = new(idempotencyKey, fromUser, fromAccount, destinationType, destination, amount);
            transaction.AddDomainEvent(new TransactionCreatedDomainEvent(transaction));
            return transaction;
        }

        public void Complete()
        {
            EnsurePending();
            Status = TransactionStatus.Completed;
            CompletedAt = DateTime.UtcNow;

            AddDomainEvent(new TransactionCompletedDomainEvent(this));
        }

        public void Fail(string reason)
        {
            EnsurePending();

            if (string.IsNullOrWhiteSpace(reason)) throw new ValueEmptyException(nameof(reason), "A failure reason must be provided.");

            Status = TransactionStatus.Failed;
            FailureReason = reason;
            CompletedAt = DateTime.UtcNow;

            AddDomainEvent(new TransactionFailedDomainEvent(this));
        }

        private void EnsurePending()
        {
            if (Status != TransactionStatus.Pending) throw new TransactionNotPendingException(Id, Status);
        }
    }
}