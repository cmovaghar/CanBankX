using System.Net;
using System.Net.Http.Json;

namespace Frontend.Services;

public record RegisterResponse(string Token);

public class AuthService(HttpClient http, ILogger<AuthService> logger, TokenService tokenService)
{
    private readonly HttpClient _http = http;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly TokenService _tokenService = tokenService;

    public async Task<(bool Success, string? Error)> RegisterAsync(RegisterModel model)
    {
        try
        {
            var payload = new
            {
                model.Email,
                model.Password,
                model.FirstName,
                model.LastName,
                model.DateOfBirth,
                model.Street,
                model.City,
                model.Province,
                model.PostalCode,
                model.Sin,
                model.PhoneNumber
            };

            var response = await _http.PostAsJsonAsync("/api/users/register", payload);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
                await _tokenService.SetTokenAsync(result!.Token);
                _logger.LogInformation("Register token: {Token}", result!.Token);
                return (true, null);
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Registration failed: {StatusCode} - {Content}", response.StatusCode, content);
            return (false, $"Registration failed: {response.ReasonPhrase}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during registration");
            return (false, "Unable to connect to the server. Please try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration");
            return (false, "An unexpected error occurred.");
        }
    }

    public async Task<(bool Success, string? Error)> LoginAsync(LoginModel model)
    {
        try
        {
            var payload = new
            {
                model.Email,
                model.Password
            };

            var response = await _http.PostAsJsonAsync("/api/users/login", payload);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
                await _tokenService.SetTokenAsync(result!.Token);
                _logger.LogInformation("Login token: {Token}", result!.Token);
                return (true, null);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return (false, "Invalid email or password");
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Login failed: {StatusCode} - {Content}", response.StatusCode, content);
            return (false, $"Login failed: {response.ReasonPhrase}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during login");
            return (false, "Unable to connect to the server. Please try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login");
            return (false, "An unexpected error occurred.");
        }
    }

    public async Task<(bool Success, string? Error)> VerifyEmailAsync(string code)
    {
        try
        {
            var payload = new { code };

            var response = await _http.PostAsJsonAsync("/api/users/verify-email", payload);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
                await _tokenService.SetTokenAsync(result!.Token);
                _logger.LogInformation("Verify-email token: {Token}", result!.Token);
                return (true, null);
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Verification failed: {StatusCode} - {Content}", response.StatusCode, content);
            return (false, $"Verification failed: {response.ReasonPhrase}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during verification");
            return (false, "Unable to connect to the server. Please try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during verification");
            return (false, "An unexpected error occurred.");
        }
    }
}