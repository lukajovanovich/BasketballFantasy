using BasketballFantasy.API.DTOs.Coaches;
using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace BasketballFantasy.API.Controllers
{
    [ApiController]
    [Route("coaches")]
    public class CoachesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateCoachRequest> _createValidator;
        private readonly IValidator<UpdateCoachRequest> _updateValidator;

        public CoachesController(
            IUnitOfWork uow,
            IValidator<CreateCoachRequest> createValidator,
            IValidator<UpdateCoachRequest> updateValidator)
        {
            _uow = uow;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var coaches = _uow.Coaches.GetAllWithTeam()
                .Select(MapCoach)
                .ToList();

            return Ok(coaches);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var coach = _uow.Coaches.GetByIdWithTeam(id);
            if (coach == null)
            {
                return NotFound();
            }

            return Ok(MapCoach(coach));
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateCoachRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            if (_uow.Teams.GetById(request.TeamId) == null)
            {
                return BadRequest($"Tim sa Id={request.TeamId} ne postoji.");
            }

            var coach = new Coach
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                ExperienceYears = request.ExperienceYears,
                TeamId = request.TeamId
            };

            _uow.Coaches.Add(coach);
            _uow.SaveChanges();

            var createdCoach = _uow.Coaches.GetByIdWithTeam(coach.Id)!;
            return CreatedAtAction(nameof(GetById), new { id = coach.Id }, MapCoach(createdCoach));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateCoachRequest request)
        {
            var coach = _uow.Coaches.GetById(id);
            if (coach == null)
            {
                return NotFound();
            }

            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            if (_uow.Teams.GetById(request.TeamId) == null)
            {
                return BadRequest($"Tim sa Id={request.TeamId} ne postoji.");
            }

            coach.FirstName = request.FirstName;
            coach.LastName = request.LastName;
            coach.ExperienceYears = request.ExperienceYears;
            coach.TeamId = request.TeamId;

            _uow.Coaches.Update(coach);
            _uow.SaveChanges();

            var updatedCoach = _uow.Coaches.GetByIdWithTeam(id)!;
            return Ok(MapCoach(updatedCoach));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var coach = _uow.Coaches.GetById(id);
            if (coach == null)
            {
                return NotFound();
            }

            if (_uow.FantasyTeams.Find(ft => ft.CoachId == id).Any())
            {
                return BadRequest("Trener ne moze da se obrise dok je vezan za fantasy tim.");
            }

            _uow.Coaches.Remove(coach);
            _uow.SaveChanges();

            return NoContent();
        }

        private IActionResult ValidationError(ValidationResult validationResult) =>
            BadRequest(validationResult.Errors.Select(error => new { error.PropertyName, error.ErrorMessage }));

        private static CoachDto MapCoach(Coach coach) =>
            new()
            {
                Id = coach.Id,
                FirstName = coach.FirstName,
                LastName = coach.LastName,
                ExperienceYears = coach.ExperienceYears,
                TeamId = coach.TeamId,
                TeamName = coach.Team?.Name ?? string.Empty
            };
    }
}
