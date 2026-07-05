using BasketballFantasy.API.DTOs.Matches;
using FluentValidation;

namespace BasketballFantasy.API.Validators
{
    public class CreateMatchRequestValidator : AbstractValidator<CreateMatchRequest>
    {
        public CreateMatchRequestValidator()
        {
            RuleFor(x => x.HomeTeamId)
                .GreaterThan(0)
                .WithMessage("HomeTeamId mora biti veci od 0.");

            RuleFor(x => x.AwayTeamId)
                .GreaterThan(0)
                .WithMessage("AwayTeamId mora biti veci od 0.");

            RuleFor(x => x.AwayTeamId)
                .NotEqual(x => x.HomeTeamId)
                .WithMessage("Domaci i gostujuci tim moraju biti razliciti.");

            RuleFor(x => x.MatchDate)
                .NotEmpty()
                .WithMessage("Datum utakmice je obavezan.");

            RuleFor(x => x.HomeScore)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Broj poena domaceg tima ne moze biti negativan.");

            RuleFor(x => x.AwayScore)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Broj poena gostujuceg tima ne moze biti negativan.");
        }
    }

    public class UpdateMatchRequestValidator : AbstractValidator<UpdateMatchRequest>
    {
        public UpdateMatchRequestValidator()
        {
            RuleFor(x => x.HomeTeamId)
                .GreaterThan(0)
                .WithMessage("HomeTeamId mora biti veci od 0.");

            RuleFor(x => x.AwayTeamId)
                .GreaterThan(0)
                .WithMessage("AwayTeamId mora biti veci od 0.");

            RuleFor(x => x.AwayTeamId)
                .NotEqual(x => x.HomeTeamId)
                .WithMessage("Domaci i gostujuci tim moraju biti razliciti.");

            RuleFor(x => x.MatchDate)
                .NotEmpty()
                .WithMessage("Datum utakmice je obavezan.");

            RuleFor(x => x.HomeScore)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Broj poena domaceg tima ne moze biti negativan.");

            RuleFor(x => x.AwayScore)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Broj poena gostujuceg tima ne moze biti negativan.");
        }
    }
}
