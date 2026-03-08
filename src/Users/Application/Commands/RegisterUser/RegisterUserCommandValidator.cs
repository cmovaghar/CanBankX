using FluentValidation;

namespace Users.Application.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        private static readonly string[] ValidProvinces = ["AB", "BC", "MB", "NB", "NL", "NS", "NT", "NU", "ON", "PE", "QC", "SK", "YT"];

        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(254);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(12)
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

            RuleFor(x => x.DateOfBirth)
                .Must(dob => dob <= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18)))
                .WithMessage("You must be at least 18 years old to open an account.");

            RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);

            RuleFor(x => x.Province)
                .NotEmpty()
                .Must(p => ValidProvinces.Contains(p.ToUpperInvariant()))
                .WithMessage("Province must be a valid Canadian province or territory code.");

            RuleFor(x => x.PostalCode)
                .NotEmpty()
                .Matches(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$")
                .WithMessage("Postal code must be in Canadian format (e.g. A1A 1A1).");

            RuleFor(x => x.Sin)
                .NotEmpty()
                .Matches(@"^\d{9}$").WithMessage("SIN must be exactly 9 digits.");
                //.Must(BeValidSin).WithMessage("SIN is not valid.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+\d{10,15}$")
                .When(x => x.PhoneNumber is not null)
                .WithMessage("Phone number must be in E.164 format (e.g. +14165551234).");
        }

        // Algo de Luhn pour vérifier les SIN
        private static bool BeValidSin(string sin)
        {
            if (sin.Length != 9 || !sin.All(char.IsDigit)) return false;

            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                int digit = sin[i] - '0';
                if (i % 2 != 0)
                {
                    digit *= 2;
                    if (digit > 9) digit -= 9;
                }
                sum += digit;
            }
            return sum % 10 == 0;
        }
    }
}
