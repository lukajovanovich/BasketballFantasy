using BasketballFantasy.API.Auth;
using BasketballFantasy.API.DTOs.PlayerStatistics;
using BasketballFantasy.API.Features.PlayerStatistics;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasketballFantasy.API.Controllers
{
    [Authorize]
    [Route("player-statistics")]
    public class PlayerStatisticsController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IValidator<CreatePlayerStatisticRequest> _createValidator;
        private readonly IValidator<UpdatePlayerStatisticRequest> _updateValidator;

        public PlayerStatisticsController(
            ISender sender,
            IValidator<CreatePlayerStatisticRequest> createValidator,
            IValidator<UpdatePlayerStatisticRequest> updateValidator)
        {
            _sender = sender;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var statistics = await _sender.Send(new GetAllPlayerStatisticsQuery());
            return Ok(statistics);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var statistic = await _sender.Send(new GetPlayerStatisticByIdQuery(id));
            return Ok(statistic);
        }

        [HttpGet("top-scorers")]
        public async Task<IActionResult> GetTopScorers([FromQuery] int count = 10)
        {
            var topScorers = await _sender.Send(new GetTopScorersQuery(count));
            return Ok(topScorers);
        }

        [HttpPost]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreatePlayerStatisticRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var statistic = await _sender.Send(new CreatePlayerStatisticCommand(request));
            return CreatedAtAction(nameof(GetById), new { id = statistic.Id }, statistic);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlayerStatisticRequest request)
        {
            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var statistic = await _sender.Send(new UpdatePlayerStatisticCommand(id, request));
            return Ok(statistic);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await _sender.Send(new DeletePlayerStatisticCommand(id));
            return NoContent();
        }
    }
}
