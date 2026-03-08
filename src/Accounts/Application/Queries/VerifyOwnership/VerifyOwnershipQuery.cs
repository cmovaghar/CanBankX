using MediatR;

namespace Accounts.Application.Queries.VerifyOwnership
{
    public sealed record VerifyOwnershipRequest(Guid UserId, Guid AccountId);

    public sealed record VerifyOwnershipQuery(Guid UserId, Guid AccountId) : IRequest<VerifyOwnershipResponse>;

    public sealed record VerifyOwnershipResponse(bool IsOwner);
}
