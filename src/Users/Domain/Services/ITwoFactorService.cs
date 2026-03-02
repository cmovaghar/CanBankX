using Users.Domain.Entities;

namespace Users.Domain.Services
{
    public interface ITwoFactorService
    {
        Task<TotpSetupResult> GenerateTotpSecretAsync(Guid userId, string email);

        Task SendSmsOtpAsync(Guid userId, string phoneNumber, CancellationToken cancellationToken = default);

        Task<bool> ValidateOtpAsync(Guid userId, string code, TwoFactorMethod method, CancellationToken cancellationToken = default);
    }

    public record TotpSetupResult(string Secret, string ProvisioningUri);
}
