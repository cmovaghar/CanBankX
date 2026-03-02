namespace Users.Application.Requests
{
    public record RegisterUserRequest(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        DateOnly DateOfBirth,
        string Street,
        string City,
        string Province,
        string PostalCode,
        string? PhoneNumber = null);
}
