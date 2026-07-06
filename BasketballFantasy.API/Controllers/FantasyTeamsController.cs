using BasketballFantasy.API.Auth;
using BasketballFantasy.API.DTOs.FantasyTeams;
using BasketballFantasy.API.Features.FantasyTeams;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasketballFantasy.API.Controllers
{
    [Authorize]
    [Route("fantasy-teams")]
    public class FantasyTeamsController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IValidator<CreateFantasyTeamRequest> _createValidator;
        private readonly IValidator<UpdateFantasyTeamRequest> _updateValidator;
        private readonly IValidator<AddFantasyTeamPlayerRequest> _addPlayerValidator;

        public FantasyTeamsController(
            ISender sender,
            IValidator<CreateFantasyTeamRequest> createValidator,
            IValidator<UpdateFantasyTeamRequest> updateValidator,
            IValidator<AddFantasyTeamPlayerRequest> addPlayerValidator)
        {
            _sender = sender;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _addPlayerValidator = addPlayerValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var fantasyTeams = await _sender.Send(new GetAllFantasyTeamsQuery());
            return Ok(fantasyTeams);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var fantasyTeam = await _sender.Send(new GetFantasyTeamByIdQuery(id));
            return Ok(fantasyTeam);
        }

        [HttpPost]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateFantasyTeamRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var fantasyTeam = await _sender.Send(new CreateFantasyTeamCommand(request));
            return CreatedAtAction(nameof(GetById), new { id = fantasyTeam.Id }, fantasyTeam);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFantasyTeamRequest request)
        {
            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var fantasyTeam = await _sender.Send(new UpdateFantasyTeamCommand(id, request));
            return Ok(fantasyTeam);
        }

        [HttpPost("{id}/players")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> AddPlayer(int id, [FromBody] AddFantasyTeamPlayerRequest request)
        {
            var validationResult = _addPlayerValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var fantasyTeam = await _sender.Send(new AddFantasyTeamPlayerCommand(id, request));
            return CreatedAtAction(nameof(GetById), new { id }, fantasyTeam);
        }

        [HttpGet("{id}/points")]
        public async Task<IActionResult> GetPoints(int id)
        {
            var result = await _sender.Send(new GetFantasyTeamPointsQuery(id));
            return Ok(result);
        }

        [HttpGet("{id}/export")]
        public async Task<IActionResult> Export(int id)
        {
            var export = await _sender.Send(new ExportFantasyTeamQuery(id));
            return File(export.Content, export.ContentType, export.FileName);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await _sender.Send(new DeleteFantasyTeamCommand(id));
            return NoContent();
        }
    }
}
