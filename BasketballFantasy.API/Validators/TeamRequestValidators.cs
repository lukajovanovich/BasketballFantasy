using BasketballFantasy.API.DTOs.Teams;
using FluentValidation;

namespace BasketballFantasy.API.Validators
{
    public class CreateTeamRequestValidator : AbstractValidator<CreateTeamRequest>
    {
        public CreateTeamRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Naziv tima je obavezan.")
                .MaximumLength(100);

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("Grad je obavezan.")
                .MaximumLength(100);
        }
    }

    public class UpdateTeamRequestValidator : AbstractValidator<UpdateTeamRequest>
    {
        public UpdateTeamRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Naziv tima je obavezan.")
                .MaximumLength(100);

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("Grad je obavezan.")
                .MaximumLength(100);
        }
    }
}
