using MediatR;
using Transactions.Domain.Entities;
using Transactions.Domain.Repositories;

namespace Transactions.Application.Commands
{
    public class InitiateTransactionHandler(ITransactionRepository transactions) : IRequestHandler<InitiateTransactionCommand, InitiateTransactionResponse>
    {
        private readonly ITransactionRepository _transactions = transactions;

        public async Task<InitiateTransactionResponse> Handle(InitiateTransactionCommand cmd, CancellationToken ct)
        {
            var existing = await _transactions.GetByIdempotencyKeyAsync(cmd.IdempotencyKey, ct);
            if (existing is not null) return new InitiateTransactionResponse(existing, true);

            var transaction = Transaction.Initiate(cmd.IdempotencyKey, cmd.FromUser, cmd.FromAccount, cmd.DestinationType, cmd.Destination, cmd.Amount);

            _transactions.Add(transaction);
            await _transactions.UnitOfWork.SaveChangesAsync(ct);

            return new InitiateTransactionResponse(transaction, false);
        }
    }
}
