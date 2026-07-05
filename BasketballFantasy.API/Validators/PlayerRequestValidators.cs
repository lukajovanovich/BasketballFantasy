using BasketballFantasy.API.DTOs.Players;
using FluentValidation;

namespace BasketballFantasy.API.Validators
{
    public class CreatePlayerRequestValidator : AbstractValidator<CreatePlayerRequest>
    {
        public CreatePlayerRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Ime igraca je obavezno.")
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Prezime igraca je obavezno.")
                .MaximumLength(100);

            RuleFor(x => x.Position)
                .NotEmpty()
                .WithMessage("Pozicija je obavezna.")
                .MaximumLength(30);

            RuleFor(x => x.JerseyNumber)
                .GreaterThan(0)
                .WithMessage("Broj na dresu mora biti veci od 0.");

            RuleFor(x => x.Height)
                .InclusiveBetween(150m, 260m)
                .WithMessage("Visina mora biti realna vrednost izmedju 150 i 260 cm.");

            RuleFor(x => x.TeamId)
                .GreaterThan(0)
                .WithMessage("TeamId mora biti veci od 0.");
        }
    }

    public class UpdatePlayerRequestValidator : AbstractValidator<UpdatePlayerRequest>
    {
        public UpdatePlayerRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Ime igraca je obavezno.")
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Prezime igraca je obavezno.")
                .MaximumLength(100);

            RuleFor(x => x.Position)
                .NotEmpty()
                .WithMessage("Pozicija je obavezna.")
                .MaximumLength(30);

            RuleFor(x => x.JerseyNumber)
                .GreaterThan(0)
                .WithMessage("Broj na dresu mora biti veci od 0.");

            RuleFor(x => x.Height)
                .InclusiveBetween(150m, 260m)
                .WithMessage("Visina mora biti realna vrednost izmedju 150 i 260 cm.");

            RuleFor(x => x.TeamId)
                .GreaterThan(0)
                .WithMessage("TeamId mora biti veci od 0.");
        }
    }
}
