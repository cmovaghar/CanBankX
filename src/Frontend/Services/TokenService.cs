using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;

namespace Frontend.Services
{
    public class TokenService(IJSRuntime js)
    {
        private readonly IJSRuntime _js = js;

        public async Task SetTokenAsync(string token)
            => await _js.InvokeVoidAsync("localStorage.setItem", "auth_token", token);

        public async Task<string?> GetTokenAsync()
            => await _js.InvokeAsync<string?>("localStorage.getItem", "auth_token");

        public async Task RemoveTokenAsync()
            => await _js.InvokeVoidAsync("localStorage.removeItem", "auth_token");

        public static bool IsTokenValid(string? token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(token)) return false;

                var jwt = handler.ReadJwtToken(token);

                return jwt.ValidTo == DateTime.MinValue || jwt.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        public static string? GetPurpose(string token) => GetClaim(token, "purpose");

        public static string? GetClaim(string token, string claimType)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                return jwt.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}