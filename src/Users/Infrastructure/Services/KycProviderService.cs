using Users.Domain.Services;
using Users.Domain.ValueObjects;

namespace Users.Infrastructure.Services
{
    public class KycProviderService(ILogger<KycProviderService> logger) : IKycService
    {
        private readonly ILogger<KycProviderService> _logger = logger;

        public async Task<string> InitiateVerificationAsync(
            Guid userId,
            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            Address address,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("KYC verification initiated for user {UserId} — stub returns a fake reference ID.", userId);

            await Task.Delay(10, cancellationToken);
            return $"KYC-REF-{userId:N}";
        }
    }
}
