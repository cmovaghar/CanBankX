using Accounts.Domain.Entities;
using Accounts.Domain.Repositories;
using MediatR;

namespace Accounts.Application.Commands.OpenAccount
{
    public class OpenAccountHandler(IAccountRepository accounts) : IRequestHandler<OpenAccountCommand, OpenAccountResponse>
    {
        private readonly IAccountRepository _accounts = accounts;

        public async Task<OpenAccountResponse> Handle(OpenAccountCommand cmd, CancellationToken ct)
        {
            var account = Account.Open(cmd.UserId, cmd.Type, cmd.Currency);

            _accounts.Add(account);
            await _accounts.UnitOfWork.SaveChangesAsync(ct);

            return new OpenAccountResponse(account.Id, account.Type, cmd.UserId);
        }
    }
}
