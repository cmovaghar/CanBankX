using Accounts.Application.Queries.Dto;
using MediatR;

namespace Accounts.Application.Queries.GetUserAccounts
{
    public sealed record GetUserAccountsQuery(Guid UserId) : IRequest<GetUserAccountsResponse>;

    public sealed record GetUserAccountsResponse(Guid UserId, IReadOnlyList<AccountLimitedDto> Accounts);
}
