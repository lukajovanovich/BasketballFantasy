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

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Lozinka je obavezna.")
                .MinimumLength(6)
                .WithMessage("Lozinka mora imati najmanje 6 karaktera.");
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
