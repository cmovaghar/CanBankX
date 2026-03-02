using Users.Domain.Entities;
using Users.Domain.Services;

namespace Users.Infrastructure.Services
{
    public class TwoFactorService(ILogger<TwoFactorService> logger) : ITwoFactorService
    {
        private readonly ILogger<TwoFactorService> _logger = logger;

        public Task<TotpSetupResult> GenerateTotpSecretAsync(Guid userId, string email)
        {
            _logger.LogInformation("TOTP secret generated for user {UserId}.", userId);
            return Task.FromResult(new TotpSetupResult("base32Secret", "uri"));
        }

        public async Task SendSmsOtpAsync(Guid userId, string phoneNumber, CancellationToken cancellationToken = default)
        {
            var code = new Random().Next(100000, 999999).ToString();

            // TODO: store hashed code in cache (Redis) with 5-minute TTL

            _logger.LogInformation("SMS OTP sent to {PhoneNumber} for user {UserId} (stub).", phoneNumber, userId);
            await Task.CompletedTask;
        }

        public Task<bool> ValidateOtpAsync(Guid userId, string code, TwoFactorMethod method, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("OTP validation stub for user {UserId} — always returns true.", userId);
            return Task.FromResult(true);
        }
    }
}
