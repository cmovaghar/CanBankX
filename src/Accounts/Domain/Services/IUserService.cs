namespace Accounts.Domain.Services
{
    public interface IUserService
    {
        Task<Guid> GetInteracAccountForUserAsync(Guid UserId, CancellationToken ct = default);
    }
}
