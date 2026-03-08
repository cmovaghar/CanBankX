using MediatR;

namespace Users.Application.Commands.SetupInteracDeposit
{
    public sealed record SetupInteracDepositRequest(Guid AccountId);
 
    public sealed record SetupInteracDepositCommand(Guid AccountId, Guid UserId) : IRequest<SetupInteracDepositResponse>;

    public sealed record SetupInteracDepositResponse(Guid AccountId);
}
