using BasketballFantasy.API.Auth;
using BasketballFantasy.API.DTOs.Players;
using BasketballFantasy.API.Features.Players;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasketballFantasy.API.Controllers
{
    [Authorize]
    [Route("players")]
    public class PlayersController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IValidator<CreatePlayerRequest> _createValidator;
        private readonly IValidator<UpdatePlayerRequest> _updateValidator;

        public PlayersController(
            ISender sender,
            IValidator<CreatePlayerRequest> createValidator,
            IValidator<UpdatePlayerRequest> updateValidator)
        {
            _sender = sender;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var players = await _sender.Send(new GetAllPlayersQuery());
            return Ok(players);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var player = await _sender.Send(new GetPlayerByIdQuery(id));
            return Ok(player);
        }

        [HttpGet("{id}/statistics")]
        public async Task<IActionResult> GetStatistics(int id)
        {
            var statistics = await _sender.Send(new GetPlayerStatisticsQuery(id));
            return Ok(statistics);
        }

        [HttpGet("{id}/totals")]
        public async Task<IActionResult> GetTotals(int id)
        {
            var totals = await _sender.Send(new GetPlayerTotalsQuery(id));
            return Ok(totals);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlayerRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var player = await _sender.Send(new CreatePlayerCommand(request));
            return CreatedAtAction(nameof(GetById), new { id = player.Id }, player);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlayerRequest request)
        {
            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var player = await _sender.Send(new UpdatePlayerCommand(id, request));
            return Ok(player);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await _sender.Send(new DeletePlayerCommand(id));
            return NoContent();
        }
    }
}
