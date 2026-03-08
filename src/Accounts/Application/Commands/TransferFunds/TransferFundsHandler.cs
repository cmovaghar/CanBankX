using Accounts.Domain.Entities;
using Accounts.Domain.Exceptions;
using Accounts.Domain.Repositories;
using MediatR;

namespace Accounts.Application.Commands.TransferFunds
{
    public class TransferFundsHandler(IAccountRepository accounts) : IRequestHandler<TransferFundsCommand, TransferFundsResponse>
    {
        private readonly IAccountRepository _accounts = accounts;

        public async Task<TransferFundsResponse> Handle(TransferFundsCommand cmd, CancellationToken ct)
        {
            var from = await _accounts.GetByIdAsync(cmd.From, ct) ?? throw new AccountNotFoundException(cmd.From);
            if (from.Status != AccountStatus.Active) throw new AccountNotActiveException(cmd.From, from.Status);

            var to = await _accounts.GetByIdAsync(cmd.To, ct) ?? throw new AccountNotFoundException(cmd.To);
            if (to.Status != AccountStatus.Active) throw new AccountNotActiveException(cmd.To, to.Status);

            if (!from.Balance.IsGreaterThanOrEqual(cmd.Amount)) throw new InsufficientFundsException(cmd.From, cmd.Amount, from.Balance);

            from.Debit(cmd.Amount);
            to.Credit(cmd.Amount);

            _accounts.Update(from);
            _accounts.Update(to);

            await _accounts.UnitOfWork.SaveChangesAsync(ct);

            return new TransferFundsResponse(from.Id, to.Id, cmd.Amount);
        }
    }
}
