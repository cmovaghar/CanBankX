namespace Frontend.Services;

public class AuthService(HttpClient http, ILogger<AuthService> logger)
{
    private readonly HttpClient _http = http;
    private readonly ILogger<AuthService> _logger = logger;

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
                model.PhoneNumber
            };

            var response = await _http.PostAsJsonAsync("/api/users/register", payload);

            if (response.IsSuccessStatusCode)
                return (true, null);

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
}
