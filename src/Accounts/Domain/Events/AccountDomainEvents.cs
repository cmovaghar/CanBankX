using Accounts.Domain.Entities;
using Accounts.Domain.ValueObjects;
using Shared;

namespace Accounts.Domain.Events
{
    public record AccountOpenedDomainEvent(Account Account) : IDomainEvent;

    public record AccountClosedDomainEvent(Guid AccountId, Guid UserId) : IDomainEvent;

    public record AccountFrozenDomainEvent(Guid AccountId, Guid UserId, string Reason) : IDomainEvent;

    public record AccountDebitedDomainEvent(Guid AccountId, Money Amount, Money NewBalance) : IDomainEvent;

    public record AccountCreditedDomainEvent(Guid AccountId, Money Amount, Money NewBalance) : IDomainEvent;
}
