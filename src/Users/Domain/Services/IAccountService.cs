namespace Accounts.Domain.Services
{
    public interface IAccountService
    {
        Task<bool> VerifyOwnershipAsync(Guid userId, Guid accountId, CancellationToken ct);
    }
}
