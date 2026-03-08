using MediatR;

namespace Users.Application.Commands.LoginUser
{
    public sealed record LoginUserRequest(string Email, string Password);

    public sealed record LoginUserCommand(string Email, string Password) : IRequest<LoginUserResponse>;

    public sealed record LoginUserResponse(Guid UserId, string Email, string Token);
}
