using BasketballFantasy.API.Auth;
using BasketballFantasy.API.DTOs.Matches;
using BasketballFantasy.API.Features.Matches;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasketballFantasy.API.Controllers
{
    [Authorize]
    [Route("matches")]
    public class MatchesController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IValidator<CreateMatchRequest> _createValidator;
        private readonly IValidator<UpdateMatchRequest> _updateValidator;

        public MatchesController(
            ISender sender,
            IValidator<CreateMatchRequest> createValidator,
            IValidator<UpdateMatchRequest> updateValidator)
        {
            _sender = sender;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var matches = await _sender.Send(new GetAllMatchesQuery());
            return Ok(matches);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var match = await _sender.Send(new GetMatchByIdQuery(id));
            return Ok(match);
        }

        [HttpGet("{id}/statistics/export")]
        public async Task<IActionResult> ExportStatistics(int id)
        {
            var export = await _sender.Send(new ExportMatchStatisticsQuery(id));
            return File(export.Content, export.ContentType, export.FileName);
        }

        [HttpPost]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateMatchRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var match = await _sender.Send(new CreateMatchCommand(request));
            return CreatedAtAction(nameof(GetById), new { id = match.Id }, match);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMatchRequest request)
        {
            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var match = await _sender.Send(new UpdateMatchCommand(id, request));
            return Ok(match);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await _sender.Send(new DeleteMatchCommand(id));
            return NoContent();
        }
    }
}
