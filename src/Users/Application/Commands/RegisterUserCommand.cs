using MediatR;
using Users.Application.Results;

namespace Users.Application.Commands
{
    public record RegisterUserCommand(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        DateOnly DateOfBirth,
        string Street,
        string City,
        string Province,
        string PostalCode,
        string? PhoneNumber = null) : IRequest<RegisterUserResult>;
}
