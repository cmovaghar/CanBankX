using Accounts.Domain.Events;
using Accounts.Domain.Exceptions;
using Accounts.Domain.ValueObjects;
using Shared;

namespace Accounts.Domain.Entities
{
    public class Account : Entity, IAggregateRoot
    {
        public Guid UserId { get; private set; }
        public AccountType Type { get; private set; }
        public AccountStatus Status { get; private set; }
        public Money Balance { get; private set; } = null!;
        public string NickName { get; private set; } = string.Empty;
        public DateTime OpenedAt { get; private set; }
        public DateTime? ClosedAt { get; private set; }

        private Account() { }

        private Account(Guid id, Guid userId, AccountType type, Money initialBalance)
        {
            Id = id;
            UserId = userId;
            Type = type;
            Status = AccountStatus.Active;
            Balance = initialBalance;
            OpenedAt = DateTime.UtcNow;
        }

        public static Account Open(Guid userId, AccountType type, Currency currency)
        {
            var id = Guid.NewGuid();
            var initialBalance = Money.Of(100, currency);

            Account account = new(id, userId, type, initialBalance);
            account.AddDomainEvent(new AccountOpenedDomainEvent(account));
            return account;
        }

        public void Credit(Money amount)
        {
            EnsureActive();
            if (amount.Currency != Balance.Currency) throw new CurrenciesMissmatchException(Balance.Currency, amount.Currency);

            Balance = Balance.Add(amount);
            AddDomainEvent(new AccountCreditedDomainEvent(Id, amount, Balance));
        }

        public void Debit(Money amount)
        {
            EnsureActive();
            if (amount.Currency != Balance.Currency) throw new CurrenciesMissmatchException(Balance.Currency, amount.Currency);
            if (Balance.Amount < amount.Amount) throw new InsufficientFundsException(Id, Balance, amount);

            Balance = Balance.Subtract(amount);
            AddDomainEvent(new AccountDebitedDomainEvent(Id, amount, Balance));
        }

        public void Freeze(string reason)
        {
            if (Status == AccountStatus.Frozen) return;

            Status = AccountStatus.Frozen;
            AddDomainEvent(new AccountFrozenDomainEvent(Id, UserId, reason));
        }

        public void Close()
        {
            EnsureActive();
            if (Balance.Amount != 0) throw new AccountNotEmptyException(Id, Balance);

            Status = AccountStatus.Closed;
            ClosedAt = DateTime.UtcNow;
            AddDomainEvent(new AccountClosedDomainEvent(Id, UserId));
        }

        public void UpdateNickName(string nickName)
        {
            NickName = nickName.Trim();
        }

        private void EnsureActive()
        {
            if (Status != AccountStatus.Active) throw new AccountNotActiveException(Id, Status);
        }
    }
}
