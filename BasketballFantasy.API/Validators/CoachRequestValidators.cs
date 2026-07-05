using BasketballFantasy.API.DTOs.Coaches;
using FluentValidation;

namespace BasketballFantasy.API.Validators
{
    public class CreateCoachRequestValidator : AbstractValidator<CreateCoachRequest>
    {
        public CreateCoachRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Ime trenera je obavezno.")
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Prezime trenera je obavezno.")
                .MaximumLength(100);

            RuleFor(x => x.ExperienceYears)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Godine iskustva ne mogu biti negativne.");

            RuleFor(x => x.TeamId)
                .GreaterThan(0)
                .WithMessage("TeamId mora biti veci od 0.");
        }
    }

    public class UpdateCoachRequestValidator : AbstractValidator<UpdateCoachRequest>
    {
        public UpdateCoachRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Ime trenera je obavezno.")
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Prezime trenera je obavezno.")
                .MaximumLength(100);

            RuleFor(x => x.ExperienceYears)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Godine iskustva ne mogu biti negativne.");

            RuleFor(x => x.TeamId)
                .GreaterThan(0)
                .WithMessage("TeamId mora biti veci od 0.");
        }
    }
}
