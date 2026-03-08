using MediatR;

namespace Users.Application.Queries.GetInteracAccount
{
    public sealed record GetInteracAccountRequest(Guid UserId);

    public sealed record GetInteracAccountQuery(Guid UserId) : IRequest<GetInteracAccountResponse>;

    public sealed record GetInteracAccountResponse(Guid AccountId);
}
