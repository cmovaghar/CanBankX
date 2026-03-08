using Accounts.Domain.Services;

namespace Accounts.Infrastructure.Services
{
    public sealed class AccountService(HttpClient http) : IAccountService
    {
        private readonly HttpClient _http = http;

        public async Task<bool> VerifyOwnershipAsync(Guid userId, Guid accountId, CancellationToken ct)
        {
            var response = await _http.PostAsJsonAsync("/api/accounts/verify-ownership", new
            {
                UserId = userId,
                AccountId = accountId
            }, ct);

            var result = await response.Content.ReadAsStringAsync(ct);

            if (result == "true") return true;
            return false;
        }
    }
}
