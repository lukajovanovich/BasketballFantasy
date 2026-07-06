using BasketballFantasy.API.Auth;
using BasketballFantasy.API.DTOs.Injuries;
using BasketballFantasy.API.Features.Injuries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasketballFantasy.API.Controllers
{
    [Authorize]
    [Route("injuries")]
    public class InjuriesController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IValidator<CreateInjuryRequest> _createValidator;
        private readonly IValidator<UpdateInjuryRequest> _updateValidator;

        public InjuriesController(
            ISender sender,
            IValidator<CreateInjuryRequest> createValidator,
            IValidator<UpdateInjuryRequest> updateValidator)
        {
            _sender = sender;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var injuries = await _sender.Send(new GetAllInjuriesQuery());
            return Ok(injuries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var injury = await _sender.Send(new GetInjuryByIdQuery(id));
            return Ok(injury);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var injuries = await _sender.Send(new GetActiveInjuriesQuery());
            return Ok(injuries);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInjuryRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var injury = await _sender.Send(new CreateInjuryCommand(request));
            return CreatedAtAction(nameof(GetById), new { id = injury.Id }, injury);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInjuryRequest request)
        {
            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var injury = await _sender.Send(new UpdateInjuryCommand(id, request));
            return Ok(injury);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await _sender.Send(new DeleteInjuryCommand(id));
            return NoContent();
        }
    }
}
