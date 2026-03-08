using FluentValidation;

namespace Accounts.Application.Commands.OpenAccount
{
    public sealed class OpenAccountCommandValidator : AbstractValidator<OpenAccountCommand>
    {
        public OpenAccountCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.Currency).IsInEnum();
        }
    }
}
