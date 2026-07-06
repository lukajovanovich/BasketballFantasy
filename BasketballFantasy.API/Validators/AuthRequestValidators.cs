using BasketballFantasy.API.DTOs.Auth;
using FluentValidation;

namespace BasketballFantasy.API.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Korisnicko ime je obavezno.")
                .MinimumLength(3)
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email je obavezan.")
                .EmailAddress()
                .WithMessage("Email nije u dobrom formatu.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Lozinka je obavezna.")
                .MinimumLength(6)
                .WithMessage("Lozinka mora imati najmanje 6 karaktera.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Ime je obavezno.")
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Prezime je obavezno.")
                .MaximumLength(100);
        }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Korisnicko ime je obavezno.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Lozinka je obavezna.");
        }
    }
}
