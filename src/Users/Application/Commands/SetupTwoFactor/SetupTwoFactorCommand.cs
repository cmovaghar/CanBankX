using MediatR;
using Users.Domain.Entities;

namespace Users.Application.Commands.SetupTwoFactor
{
    public record SetupTwoFactorCommand(Guid UserId, TwoFactorMethod Method) : IRequest<SetupTwoFactorResult>;

    public record SetupTwoFactorResult(TwoFactorMethod Method, string? ProvisioningUri = null);
}
