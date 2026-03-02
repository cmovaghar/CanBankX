using Users.Domain.Entities;

namespace Users.Application.Results
{
    public record SetupTwoFactorResult(
        TwoFactorMethod Method,
        string? ProvisioningUri = null
    );
}
