using Accounts.Domain.Services;
using MediatR;
using Users.Domain.Exceptions;
using Users.Domain.Repositories;

namespace Users.Application.Commands.SetupInteracDeposit
{
    public class SetupInteracDespositHandler(IUserRepository users, IAccountService accountService) : IRequestHandler<SetupInteracDepositCommand, SetupInteracDepositResponse>
    {
        private readonly IUserRepository _users = users;
        private readonly IAccountService _accountService = accountService;

        public async Task<SetupInteracDepositResponse> Handle(SetupInteracDepositCommand cmd, CancellationToken ct)
        {
            bool isOwner = await _accountService.VerifyOwnershipAsync(cmd.UserId, cmd.AccountId, ct);

            if (!isOwner) throw new UserDoesNotOwnAccountException(cmd.UserId, cmd.AccountId);

            var user = await _users.FindByIdAsync(cmd.UserId, ct) ?? throw new UserNotFoundException(cmd.UserId);

            user.SetInteracDepositAccount(cmd.AccountId);

            _users.Update(user);
            await _users.UnitOfWork.SaveChangesAsync(ct);

            return new SetupInteracDepositResponse(cmd.AccountId);
        }
    }
}
