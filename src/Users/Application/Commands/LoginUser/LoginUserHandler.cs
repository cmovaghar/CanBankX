using MediatR;
using Users.Domain.Exceptions;
using Users.Domain.Repositories;
using Users.Domain.Services;

namespace Users.Application.Commands.LoginUser
{
    public class LoginUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtService jwtService) : IRequestHandler<LoginUserCommand, LoginUserResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IJwtService _jwtService = jwtService;

        public async Task<LoginUserResponse> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByEmailAsync(command.Email, cancellationToken);

            if (user is null || !_passwordHasher.Verify(command.Password, user.Password.Hash)) throw new UserFailedLoginException();

            var emailVerified = user.Status != Domain.Entities.UserStatus.PendingEmailVerification;
            var token = _jwtService.GenerateLoginToken(user.Id, command.Email, emailVerified);

            return new LoginUserResponse(user.Id, user.Email.Value, token);
        }
    }
}
