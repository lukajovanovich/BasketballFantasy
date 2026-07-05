using BasketballFantasy.API.DTOs.PlayerStatistics;
using FluentValidation;

namespace BasketballFantasy.API.Validators
{
    public class CreatePlayerStatisticRequestValidator : AbstractValidator<CreatePlayerStatisticRequest>
    {
        public CreatePlayerStatisticRequestValidator()
        {
            RuleFor(x => x.PlayerId)
                .GreaterThan(0)
                .WithMessage("PlayerId mora biti veci od 0.");

            RuleFor(x => x.MatchId)
                .GreaterThan(0)
                .WithMessage("MatchId mora biti veci od 0.");

            RuleFor(x => x.Points)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Poeni ne mogu biti negativni.");

            RuleFor(x => x.Assists)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Asistencije ne mogu biti negativne.");

            RuleFor(x => x.Rebounds)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Skokovi ne mogu biti negativni.");

            RuleFor(x => x.Minutes)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minuti ne mogu biti negativni.");

            RuleFor(x => x.Steals)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Ukradene lopte ne mogu biti negativne.");

            RuleFor(x => x.Blocks)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Blokade ne mogu biti negativne.");

            RuleFor(x => x.Turnovers)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Izgubljene lopte ne mogu biti negativne.");
        }
    }

    public class UpdatePlayerStatisticRequestValidator : AbstractValidator<UpdatePlayerStatisticRequest>
    {
        public UpdatePlayerStatisticRequestValidator()
        {
            RuleFor(x => x.PlayerId)
                .GreaterThan(0)
                .WithMessage("PlayerId mora biti veci od 0.");

            RuleFor(x => x.MatchId)
                .GreaterThan(0)
                .WithMessage("MatchId mora biti veci od 0.");

            RuleFor(x => x.Points)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Poeni ne mogu biti negativni.");

            RuleFor(x => x.Assists)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Asistencije ne mogu biti negativne.");

            RuleFor(x => x.Rebounds)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Skokovi ne mogu biti negativni.");

            RuleFor(x => x.Minutes)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minuti ne mogu biti negativni.");

            RuleFor(x => x.Steals)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Ukradene lopte ne mogu biti negativne.");

            RuleFor(x => x.Blocks)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Blokade ne mogu biti negativne.");

            RuleFor(x => x.Turnovers)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Izgubljene lopte ne mogu biti negativne.");
        }
    }
}
