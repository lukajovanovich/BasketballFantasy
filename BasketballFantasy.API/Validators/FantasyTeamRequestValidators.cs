using BasketballFantasy.API.DTOs.FantasyTeams;
using FluentValidation;

namespace BasketballFantasy.API.Validators
{
    public class CreateFantasyTeamRequestValidator : AbstractValidator<CreateFantasyTeamRequest>
    {
        public CreateFantasyTeamRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Naziv fantasy tima je obavezan.")
                .MaximumLength(100);

            RuleFor(x => x.CoachId)
                .GreaterThan(0)
                .WithMessage("CoachId mora biti veci od 0.");
        }
    }

    public class UpdateFantasyTeamRequestValidator : AbstractValidator<UpdateFantasyTeamRequest>
    {
        public UpdateFantasyTeamRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Naziv fantasy tima je obavezan.")
                .MaximumLength(100);

            RuleFor(x => x.CoachId)
                .GreaterThan(0)
                .WithMessage("CoachId mora biti veci od 0.");
        }
    }

    public class AddFantasyTeamPlayerRequestValidator : AbstractValidator<AddFantasyTeamPlayerRequest>
    {
        public AddFantasyTeamPlayerRequestValidator()
        {
            RuleFor(x => x.PlayerId)
                .GreaterThan(0)
                .WithMessage("PlayerId mora biti veci od 0.");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Uloga mora biti Starter ili Bench.");
        }
    }
}
