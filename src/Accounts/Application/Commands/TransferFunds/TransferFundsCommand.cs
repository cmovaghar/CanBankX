using Accounts.Domain.Entities;
using Accounts.Domain.ValueObjects;
using MediatR;

namespace Accounts.Application.Commands.TransferFunds
{
    public record TransferFundsRequest(Guid From, Guid To, decimal Amount, Currency Currency);

    public record TransferFundsCommand(Guid From, Guid To, Money Amount) : IRequest<TransferFundsResponse>;

    public record TransferFundsResponse(Guid From, Guid To, Money Amount);
}
