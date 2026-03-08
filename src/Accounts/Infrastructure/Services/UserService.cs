using Accounts.Domain.Services;
using static System.Net.WebRequestMethods;

namespace Accounts.Infrastructure.Services
{
    public class UserService(HttpClient http) : IUserService
    {
        private readonly HttpClient _http = http;

        public async Task<Guid> GetInteracAccountForUserAsync(Guid UserId, CancellationToken ct = default)
        {
            var response = await _http.PostAsJsonAsync("/api/users/verify-ownership", new
            {
                UserId
            }, ct);

            var result = await response.Content.ReadAsStringAsync(ct);
            if (!Guid.TryParse(result, out var accountId)) throw new Exception("Failed to get Interac account for user.");

            return accountId;
        }
    }
}
