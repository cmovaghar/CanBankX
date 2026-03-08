using MediatR;
using Transactions.Domain.Entities;
using Transactions.Domain.ValueObjects;

namespace Transactions.Application.Commands
{
    public sealed record InitiateTransactionRequest(
        Guid IdempotencyKey,
        Guid FromAccount,
        TransactionDestinationType DestinationType,
        string Destination,
        decimal Amount,
        Currency Currency);

    public sealed record InitiateTransactionCommand(
        Guid IdempotencyKey,
        Guid FromUser,
        Guid FromAccount,
        TransactionDestinationType DestinationType, 
        string Destination,
        Money Amount) : IRequest<InitiateTransactionResponse>;

    public sealed record InitiateTransactionResponse(Transaction Transaction, bool AlreadyExists);
}
