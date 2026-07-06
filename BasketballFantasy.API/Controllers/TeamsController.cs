using BasketballFantasy.API.Auth;
using BasketballFantasy.API.DTOs.Teams;
using BasketballFantasy.API.Features.Teams;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasketballFantasy.API.Controllers
{
    [Authorize]
    [Route("teams")]
    public class TeamsController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IValidator<CreateTeamRequest> _createValidator;
        private readonly IValidator<UpdateTeamRequest> _updateValidator;

        public TeamsController(
            ISender sender,
            IValidator<CreateTeamRequest> createValidator,
            IValidator<UpdateTeamRequest> updateValidator)
        {
            _sender = sender;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _sender.Send(new GetAllTeamsQuery());
            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var team = await _sender.Send(new GetTeamByIdQuery(id));
            return Ok(team);
        }

        [HttpGet("{id}/players")]
        public async Task<IActionResult> GetPlayers(int id)
        {
            var players = await _sender.Send(new GetTeamPlayersQuery(id));
            return Ok(players);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTeamRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var team = await _sender.Send(new CreateTeamCommand(request));
            return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTeamRequest request)
        {
            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var team = await _sender.Send(new UpdateTeamCommand(id, request));
            return Ok(team);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await _sender.Send(new DeleteTeamCommand(id));
            return NoContent();
        }
    }
}
