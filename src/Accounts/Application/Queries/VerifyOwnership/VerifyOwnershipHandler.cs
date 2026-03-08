using Accounts.Domain.Exceptions;
using Accounts.Domain.Repositories;
using MediatR;

namespace Accounts.Application.Queries.VerifyOwnership
{
    public class VerifyOwnershipHandler(IAccountRepository accounts) : IRequestHandler<VerifyOwnershipQuery, VerifyOwnershipResponse>
    {
        private readonly IAccountRepository _accounts = accounts;

        public async Task<VerifyOwnershipResponse> Handle(VerifyOwnershipQuery query, CancellationToken ct)
        {
            var account = await _accounts.GetByIdAsync(query.AccountId, ct) ?? throw new AccountNotFoundException(query.AccountId);

            return new VerifyOwnershipResponse(account.UserId == query.UserId);
        }
    }
}
