using MediatR;
using Users.Domain.Entities;
using Users.Domain.Exceptions;
using Users.Domain.Repositories;
using Users.Domain.Services;

namespace Users.Application.Commands.SetupTwoFactor
{
    public class SetupTwoFactorHandler(
        IUserRepository userRepository,
        ITwoFactorService twoFactorService) : IRequestHandler<SetupTwoFactorCommand, SetupTwoFactorResult>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITwoFactorService _twoFactorService = twoFactorService;

        public async Task<SetupTwoFactorResult> Handle(SetupTwoFactorCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByIdAsync(command.UserId, cancellationToken) ?? throw new UserNotFoundException(command.UserId);

            user.EnableTwoFactor(command.Method);

            _userRepository.Update(user);
            await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (command.Method == TwoFactorMethod.Totp)
            {
                var setup = await _twoFactorService.GenerateTotpSecretAsync(user.Id, user.Email.Value);
                return new SetupTwoFactorResult(TwoFactorMethod.Totp, setup.ProvisioningUri);
            }

            await _twoFactorService.SendSmsOtpAsync(user.Id, user.PhoneNumber!.Value, cancellationToken);
            return new SetupTwoFactorResult(TwoFactorMethod.Sms);
        }
    }
}
