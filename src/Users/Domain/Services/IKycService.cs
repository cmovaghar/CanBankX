using Users.Domain.ValueObjects;

namespace Users.Domain.Services
{
    public interface IKycService
    {
        Task<string> InitiateVerificationAsync(
            Guid userId,
            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            Address address,
            CancellationToken cancellationToken = default);
    }
}
