using MediatR;
using Users.Application.Commands;
using Users.Application.Results;
using Users.Domain.Entities;
using Users.Domain.Exceptions;
using Users.Domain.Repositories;
using Users.Domain.Services;
using Users.Domain.ValueObjects;

namespace Users.Application.Handlers
{
    public class RegisterUserHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher) : IRequestHandler<RegisterUserCommand, RegisterUserResult>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<RegisterUserResult> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            if (await _userRepository.ExistsByEmailAsync(command.Email, cancellationToken)) throw new UserDomainException($"An account with email '{command.Email}' already exists.");

            var email = new Email(command.Email);
            var password = new HashedPassword(_passwordHasher.Hash(command.Password));
            var fullName = new FullName(command.FirstName, command.LastName);
            var address = new Address(command.Street, command.City, command.Province, command.PostalCode);
            var phone = command.PhoneNumber is not null ? new PhoneNumber(command.PhoneNumber) : null;

            var user = User.Register(email, password, fullName, command.DateOfBirth, address, phone);

            _userRepository.Add(user);

            await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return new RegisterUserResult(user.Id, email.Value);
        }
    }
}
