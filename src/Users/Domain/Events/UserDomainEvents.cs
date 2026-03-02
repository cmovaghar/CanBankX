using Shared;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;

namespace Users.Domain.Events
{
    public record UserRegisteredDomainEvent(
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        DateOnly DateOfBirth,
        Address Address) : IDomainEvent;

    public record KycApprovedDomainEvent(Guid UserId) : IDomainEvent;

    public record KycRejectedDomainEvent(Guid UserId, string Reason) : IDomainEvent;

    public record TwoFactorEnabledDomainEvent(
        Guid UserId,
        TwoFactorMethod Method) : IDomainEvent;
}
