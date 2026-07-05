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
    [Route("player-statistics")]
    public class PlayerStatisticsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreatePlayerStatisticRequest> _createValidator;
        private readonly IValidator<UpdatePlayerStatisticRequest> _updateValidator;
        private readonly FantasyPointsService _fantasyPointsService;

        public PlayerStatisticsController(
            IUnitOfWork uow,
            IValidator<CreatePlayerStatisticRequest> createValidator,
            IValidator<UpdatePlayerStatisticRequest> updateValidator,
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
            var statistics = _uow.PlayerStatistics.GetAllDetailed()
                .Select(MapStatistic)
                .ToList();

            return Ok(statistics);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var statistic = _uow.PlayerStatistics.GetByIdDetailed(id);
            if (statistic == null)
            {
                return NotFound();
            }

            return Ok(MapStatistic(statistic));
        }

        [HttpGet("top-scorers")]
        public IActionResult GetTopScorers([FromQuery] int count = 10)
        {
            if (count <= 0)
            {
                return BadRequest("Count mora biti veci od 0.");
            }

            var topScorers = _uow.PlayerStatistics.GetAllDetailed()
                .GroupBy(ps => new
                {
                    ps.PlayerId,
                    FirstName = ps.Player?.FirstName ?? string.Empty,
                    LastName = ps.Player?.LastName ?? string.Empty,
                    TeamName = ps.Player?.Team?.Name ?? string.Empty
                })
                .Select(group => new TopScorerDto
                {
                    PlayerId = group.Key.PlayerId,
                    PlayerName = $"{group.Key.FirstName} {group.Key.LastName}".Trim(),
                    TeamName = group.Key.TeamName,
                    GamesPlayed = group.Count(),
                    TotalPoints = group.Sum(x => x.Points)
                })
                .OrderByDescending(x => x.TotalPoints)
                .Take(count)
                .ToList();

            return Ok(topScorers);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreatePlayerStatisticRequest request)
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

            if (_uow.Matches.GetById(request.MatchId) == null)
            {
                return BadRequest($"Utakmica sa Id={request.MatchId} ne postoji.");
            }

            if (_uow.PlayerStatistics.Find(ps => ps.PlayerId == request.PlayerId && ps.MatchId == request.MatchId).Any())
            {
                return BadRequest("Statistika za ovog igraca i ovu utakmicu vec postoji.");
            }

            var statistic = new PlayerStatistic
            {
                PlayerId = request.PlayerId,
                MatchId = request.MatchId,
                Points = request.Points,
                Assists = request.Assists,
                Rebounds = request.Rebounds,
                Minutes = request.Minutes,
                Steals = request.Steals,
                Blocks = request.Blocks,
                Turnovers = request.Turnovers
            };

            _uow.PlayerStatistics.Add(statistic);
            _uow.SaveChanges();

            var createdStatistic = _uow.PlayerStatistics.GetByIdDetailed(statistic.Id)!;
            return CreatedAtAction(nameof(GetById), new { id = statistic.Id }, MapStatistic(createdStatistic));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdatePlayerStatisticRequest request)
        {
            var statistic = _uow.PlayerStatistics.GetById(id);
            if (statistic == null)
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

            if (_uow.Matches.GetById(request.MatchId) == null)
            {
                return BadRequest($"Utakmica sa Id={request.MatchId} ne postoji.");
            }

            if (_uow.PlayerStatistics.Find(ps =>
                    ps.PlayerId == request.PlayerId &&
                    ps.MatchId == request.MatchId &&
                    ps.Id != id).Any())
            {
                return BadRequest("Statistika za ovog igraca i ovu utakmicu vec postoji.");
            }

            statistic.PlayerId = request.PlayerId;
            statistic.MatchId = request.MatchId;
            statistic.Points = request.Points;
            statistic.Assists = request.Assists;
            statistic.Rebounds = request.Rebounds;
            statistic.Minutes = request.Minutes;
            statistic.Steals = request.Steals;
            statistic.Blocks = request.Blocks;
            statistic.Turnovers = request.Turnovers;

            _uow.PlayerStatistics.Update(statistic);
            _uow.SaveChanges();

            var updatedStatistic = _uow.PlayerStatistics.GetByIdDetailed(id)!;
            return Ok(MapStatistic(updatedStatistic));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var statistic = _uow.PlayerStatistics.GetById(id);
            if (statistic == null)
            {
                return NotFound();
            }

            _uow.PlayerStatistics.Remove(statistic);
            _uow.SaveChanges();

            return NoContent();
        }

        private IActionResult ValidationError(ValidationResult validationResult) =>
            BadRequest(validationResult.Errors.Select(error => new { error.PropertyName, error.ErrorMessage }));

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
