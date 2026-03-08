using Shared;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;

namespace Users.Domain.Events
{
    public record UserRegisteredDomainEvent(
        Guid UserId,
        string Email,
        string EmailVerificationCode,
        string FirstName,
        string LastName,
        DateOnly DateOfBirth,
        Address Address) : IDomainEvent;

    public record EmailVerifiedDomainEvent(Guid UserId, DateTime VerifiedAt) : IDomainEvent;

    public record KycStartedDomainEvent(Guid UserId, DateTime StartedAt) : IDomainEvent;

    public record KycApprovedDomainEvent(Guid UserId, DateTime ApprovedAt) : IDomainEvent;

    public record KycRejectedDomainEvent(Guid UserId, DateTime RejectedAt, string Reason) : IDomainEvent;

    public record TwoFactorEnabledDomainEvent(Guid UserId, DateTime EnabledAt, TwoFactorMethod Method) : IDomainEvent;

    public record TwoFactorDisabledDomainEvent(Guid UserId, DateTime DisabledAt) : IDomainEvent;
    
    public record InteracDepositAccountChanged(Guid AccountId) : IDomainEvent;
}
