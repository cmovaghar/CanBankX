using MediatR;
using Users.Application.Results;
using Users.Domain.Entities;

namespace Users.Application.Commands
{
    public record SetupTwoFactorCommand(
        Guid UserId,
        TwoFactorMethod Method) : IRequest<SetupTwoFactorResult>;
}
