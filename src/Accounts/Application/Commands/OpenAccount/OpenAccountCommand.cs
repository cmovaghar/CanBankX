using Accounts.Domain.Entities;
using MediatR;

namespace Accounts.Application.Commands.OpenAccount
{
    public sealed record OpenAccountRequest(AccountType Type);

    public sealed record OpenAccountCommand(Guid UserId, AccountType Type, Currency Currency) : IRequest<OpenAccountResponse>;

    public sealed record OpenAccountResponse(Guid AccountId, AccountType Type, Guid UserId);
}
