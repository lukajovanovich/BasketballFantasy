using BasketballFantasy.API.DTOs.Players;
using BasketballFantasy.API.DTOs.PlayerStatistics;
using BasketballFantasy.API.Services;
using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace BasketballFantasy.API.Controllers
{
    [ApiController]
    [Route("players")]
    public class PlayersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreatePlayerRequest> _createValidator;
        private readonly IValidator<UpdatePlayerRequest> _updateValidator;
        private readonly FantasyPointsService _fantasyPointsService;

        public PlayersController(
            IUnitOfWork uow,
            IValidator<CreatePlayerRequest> createValidator,
            IValidator<UpdatePlayerRequest> updateValidator,
            FantasyPointsService fantasyPointsService)
        {
            _uow = uow;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _fantasyPointsService = fantasyPointsService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var players = _uow.Players.GetAllWithTeam()
                .Select(MapPlayer)
                .ToList();

            return Ok(players);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var player = _uow.Players.GetByIdWithTeam(id);
            if (player == null)
            {
                return NotFound();
            }

            return Ok(MapPlayer(player));
        }

        [HttpGet("{id}/statistics")]
        public IActionResult GetStatistics(int id)
        {
            if (_uow.Players.GetById(id) == null)
            {
                return NotFound();
            }

            var statistics = _uow.PlayerStatistics.GetByPlayerId(id)
                .Select(MapStatistic)
                .ToList();

            return Ok(statistics);
        }

        [HttpGet("{id}/totals")]
        public IActionResult GetTotals(int id)
        {
            var player = _uow.Players.GetByIdWithStatistics(id);
            if (player == null)
            {
                return NotFound();
            }

            var statistics = player.PlayerStatistics;

            var totals = new PlayerTotalsDto
            {
                PlayerId = player.Id,
                PlayerName = $"{player.FirstName} {player.LastName}",
                GamesPlayed = statistics.Count,
                Points = statistics.Sum(x => x.Points),
                Assists = statistics.Sum(x => x.Assists),
                Rebounds = statistics.Sum(x => x.Rebounds),
                Minutes = statistics.Sum(x => x.Minutes),
                Steals = statistics.Sum(x => x.Steals),
                Blocks = statistics.Sum(x => x.Blocks),
                Turnovers = statistics.Sum(x => x.Turnovers),
                FantasyPoints = _fantasyPointsService.CalculateTotalFantasyPoints(statistics)
            };

            return Ok(totals);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreatePlayerRequest request)
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

            var player = new Player
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Position = request.Position,
                Height = request.Height,
                JerseyNumber = request.JerseyNumber,
                TeamId = request.TeamId
            };

            _uow.Players.Add(player);
            _uow.SaveChanges();

            var createdPlayer = _uow.Players.GetByIdWithTeam(player.Id)!;
            return CreatedAtAction(nameof(GetById), new { id = player.Id }, MapPlayer(createdPlayer));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdatePlayerRequest request)
        {
            var player = _uow.Players.GetById(id);
            if (player == null)
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

            player.FirstName = request.FirstName;
            player.LastName = request.LastName;
            player.Position = request.Position;
            player.Height = request.Height;
            player.JerseyNumber = request.JerseyNumber;
            player.TeamId = request.TeamId;

            _uow.Players.Update(player);
            _uow.SaveChanges();

            var updatedPlayer = _uow.Players.GetByIdWithTeam(id)!;
            return Ok(MapPlayer(updatedPlayer));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var player = _uow.Players.GetById(id);
            if (player == null)
            {
                return NotFound();
            }

            _uow.Players.Remove(player);
            _uow.SaveChanges();

            return NoContent();
        }

        private IActionResult ValidationError(ValidationResult validationResult) =>
            BadRequest(validationResult.Errors.Select(error => new { error.PropertyName, error.ErrorMessage }));

        private static PlayerDto MapPlayer(Player player) =>
            new()
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Position = player.Position,
                Height = player.Height,
                JerseyNumber = player.JerseyNumber,
                TeamId = player.TeamId,
                TeamName = player.Team?.Name ?? string.Empty
            };

        private PlayerStatisticDto MapStatistic(PlayerStatistic statistic) =>
            new()
            {
                Id = statistic.Id,
                PlayerId = statistic.PlayerId,
                PlayerName = $"{statistic.Player?.FirstName} {statistic.Player?.LastName}".Trim(),
                MatchId = statistic.MatchId,
                MatchDate = statistic.Match?.MatchDate ?? DateTime.MinValue,
                Points = statistic.Points,
                Assists = statistic.Assists,
                Rebounds = statistic.Rebounds,
                Minutes = statistic.Minutes,
                Steals = statistic.Steals,
                Blocks = statistic.Blocks,
                Turnovers = statistic.Turnovers,
                FantasyPoints = _fantasyPointsService.CalculateFantasyPoints(statistic)
            };
    }
}
