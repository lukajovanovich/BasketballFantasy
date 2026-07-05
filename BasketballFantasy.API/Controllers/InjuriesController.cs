using BasketballFantasy.API.DTOs.Injuries;
using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace BasketballFantasy.API.Controllers
{
    [ApiController]
    [Route("injuries")]
    public class InjuriesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateInjuryRequest> _createValidator;
        private readonly IValidator<UpdateInjuryRequest> _updateValidator;

        public InjuriesController(
            IUnitOfWork uow,
            IValidator<CreateInjuryRequest> createValidator,
            IValidator<UpdateInjuryRequest> updateValidator)
        {
            _uow = uow;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var injuries = _uow.Injuries.GetAllWithPlayer()
                .Select(MapInjury)
                .ToList();

            return Ok(injuries);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var injury = _uow.Injuries.GetByIdWithPlayer(id);
            if (injury == null)
            {
                return NotFound();
            }

            return Ok(MapInjury(injury));
        }

        [HttpGet("active")]
        public IActionResult GetActive()
        {
            var injuries = _uow.Injuries.GetActiveInjuries()
                .Select(MapInjury)
                .ToList();

            return Ok(injuries);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateInjuryRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            if (_uow.Players.GetById(request.PlayerId) == null)
            {
                return BadRequest($"Igrac sa Id={request.PlayerId} ne postoji.");
            }

            var injury = new Injury
            {
                PlayerId = request.PlayerId,
                Description = request.Description,
                InjuryDate = request.InjuryDate,
                RecoveryDate = request.RecoveryDate,
                IsActive = request.IsActive
            };

            _uow.Injuries.Add(injury);
            _uow.SaveChanges();

            var createdInjury = _uow.Injuries.GetByIdWithPlayer(injury.Id)!;
            return CreatedAtAction(nameof(GetById), new { id = injury.Id }, MapInjury(createdInjury));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateInjuryRequest request)
        {
            var injury = _uow.Injuries.GetById(id);
            if (injury == null)
            {
                return NotFound();
            }

            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            if (_uow.Players.GetById(request.PlayerId) == null)
            {
                return BadRequest($"Igrac sa Id={request.PlayerId} ne postoji.");
            }

            injury.PlayerId = request.PlayerId;
            injury.Description = request.Description;
            injury.InjuryDate = request.InjuryDate;
            injury.RecoveryDate = request.RecoveryDate;
            injury.IsActive = request.IsActive;

            _uow.Injuries.Update(injury);
            _uow.SaveChanges();

            var updatedInjury = _uow.Injuries.GetByIdWithPlayer(id)!;
            return Ok(MapInjury(updatedInjury));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var injury = _uow.Injuries.GetById(id);
            if (injury == null)
            {
                return NotFound();
            }

            _uow.Injuries.Remove(injury);
            _uow.SaveChanges();

            return NoContent();
        }

        private IActionResult ValidationError(ValidationResult validationResult) =>
            BadRequest(validationResult.Errors.Select(error => new { error.PropertyName, error.ErrorMessage }));

        private static InjuryDto MapInjury(Injury injury) =>
            new()
            {
                Id = injury.Id,
                PlayerId = injury.PlayerId,
                PlayerName = $"{injury.Player?.FirstName} {injury.Player?.LastName}".Trim(),
                Description = injury.Description,
                InjuryDate = injury.InjuryDate,
                RecoveryDate = injury.RecoveryDate,
                IsActive = injury.IsActive
            };
    }
}
