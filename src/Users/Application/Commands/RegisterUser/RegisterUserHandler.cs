using MediatR;
using Users.Domain.Entities;
using Users.Domain.Exceptions;
using Users.Domain.Repositories;
using Users.Domain.Services;
using Users.Domain.ValueObjects;

namespace Users.Application.Commands.RegisterUser
{
    public class RegisterUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ISinEncryptor sinEncryptor, IJwtService jwtService) : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
    {
        private readonly IUserRepository _users = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly ISinEncryptor _sinEncryptor = sinEncryptor;
        private readonly IJwtService _jwtService = jwtService;

        public async Task<RegisterUserResponse> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            if (await _users.ExistsByEmailAsync(command.Email, cancellationToken)) throw new UserEmailAlreadyInUseException(command.Email);

            var email = new Email(command.Email);
            var password = new HashedPassword(_passwordHasher.Hash(command.Password));
            var fullName = new FullName(command.FirstName, command.LastName);
            var address = new Address(command.Street, command.City, command.Province, command.PostalCode);
            var sin = new EncryptedSin(_sinEncryptor.Encrypt(command.Sin));
            var phone = new PhoneNumber(command.PhoneNumber);

            var user = User.Register(email, password, fullName, command.DateOfBirth, address, sin, phone);

            _users.Add(user);

            await _users.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            var token = _jwtService.GenerateEmailValidationToken(user.Id, command.Email);

            return new RegisterUserResponse(user.Id, email.Value, token);
        }
    }
}
