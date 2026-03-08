using MediatR;

namespace Users.Application.Commands.RegisterUser
{
    public sealed record RegisterUserRequest(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        DateOnly DateOfBirth,
        string Street,
        string City,
        string Province,
        string PostalCode,
        string Sin,
        string PhoneNumber);

    public sealed record RegisterUserCommand(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        DateOnly DateOfBirth,
        string Street,
        string City,
        string Province,
        string PostalCode,
        string Sin,
        string PhoneNumber) : IRequest<RegisterUserResponse>;

    public record RegisterUserResponse(Guid UserId, string Email, string Token);
}
