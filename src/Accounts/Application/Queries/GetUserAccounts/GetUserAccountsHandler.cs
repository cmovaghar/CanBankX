using Accounts.Application.Queries.Dto;
using Accounts.Domain.Repositories;
using MediatR;

namespace Accounts.Application.Queries.GetUserAccounts
{
    public sealed class GetUserAccountsHandler(IAccountRepository accounts) : IRequestHandler<GetUserAccountsQuery, GetUserAccountsResponse>
    {
        private readonly IAccountRepository _accounts = accounts;

        public async Task<GetUserAccountsResponse> Handle(GetUserAccountsQuery query, CancellationToken ct)
        {
            var accounts = await _accounts.GetActiveByUserIdAsync(query.UserId, ct);
            return new GetUserAccountsResponse(query.UserId, accounts.Select(AccountLimitedDto.From).ToList());
        }
    }
}
