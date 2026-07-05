using BasketballFantasy.API.DTOs.Injuries;
using FluentValidation;

namespace BasketballFantasy.API.Validators
{
    public class CreateInjuryRequestValidator : AbstractValidator<CreateInjuryRequest>
    {
        public CreateInjuryRequestValidator()
        {
            RuleFor(x => x.PlayerId)
                .GreaterThan(0)
                .WithMessage("PlayerId mora biti veci od 0.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Opis povrede je obavezan.")
                .MaximumLength(300);

            RuleFor(x => x.InjuryDate)
                .NotEmpty()
                .WithMessage("Datum povrede je obavezan.");

            RuleFor(x => x)
                .Must(x => !x.RecoveryDate.HasValue || x.RecoveryDate.Value >= x.InjuryDate)
                .WithMessage("Recovery date ne moze biti pre injury date.");
        }
    }

    public class UpdateInjuryRequestValidator : AbstractValidator<UpdateInjuryRequest>
    {
        public UpdateInjuryRequestValidator()
        {
            RuleFor(x => x.PlayerId)
                .GreaterThan(0)
                .WithMessage("PlayerId mora biti veci od 0.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Opis povrede je obavezan.")
                .MaximumLength(300);

            RuleFor(x => x.InjuryDate)
                .NotEmpty()
                .WithMessage("Datum povrede je obavezan.");

            RuleFor(x => x)
                .Must(x => !x.RecoveryDate.HasValue || x.RecoveryDate.Value >= x.InjuryDate)
                .WithMessage("Recovery date ne moze biti pre injury date.");
        }
    }
}
