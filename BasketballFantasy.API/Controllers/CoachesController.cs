using BasketballFantasy.API.Auth;
using BasketballFantasy.API.DTOs.Coaches;
using BasketballFantasy.API.Features.Coaches;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasketballFantasy.API.Controllers
{
    [Authorize]
    [Route("coaches")]
    public class CoachesController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IValidator<CreateCoachRequest> _createValidator;
        private readonly IValidator<UpdateCoachRequest> _updateValidator;

        public CoachesController(
            ISender sender,
            IValidator<CreateCoachRequest> createValidator,
            IValidator<UpdateCoachRequest> updateValidator)
        {
            _sender = sender;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var coaches = await _sender.Send(new GetAllCoachesQuery());
            return Ok(coaches);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var coach = await _sender.Send(new GetCoachByIdQuery(id));
            return Ok(coach);
        }

        [HttpPost]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateCoachRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var coach = await _sender.Send(new CreateCoachCommand(request));
            return CreatedAtAction(nameof(GetById), new { id = coach.Id }, coach);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCoachRequest request)
        {
            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var coach = await _sender.Send(new UpdateCoachCommand(id, request));
            return Ok(coach);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await _sender.Send(new DeleteCoachCommand(id));
            return NoContent();
        }
    }
}
