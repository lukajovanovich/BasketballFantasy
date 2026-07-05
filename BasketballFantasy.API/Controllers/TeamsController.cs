using BasketballFantasy.API.DTOs.Players;
using BasketballFantasy.API.DTOs.Teams;
using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace BasketballFantasy.API.Controllers
{
    [ApiController]
    [Route("teams")]
    public class TeamsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateTeamRequest> _createValidator;
        private readonly IValidator<UpdateTeamRequest> _updateValidator;

        public TeamsController(
            IUnitOfWork uow,
            IValidator<CreateTeamRequest> createValidator,
            IValidator<UpdateTeamRequest> updateValidator)
        {
            _uow = uow;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var teams = _uow.Teams.GetAll()
                .Select(MapTeam)
                .ToList();

            return Ok(teams);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var team = _uow.Teams.GetById(id);
            if (team == null)
            {
                return NotFound();
            }

            return Ok(MapTeam(team));
        }

        [HttpGet("{id}/players")]
        public IActionResult GetPlayers(int id)
        {
            if (_uow.Teams.GetById(id) == null)
            {
                return NotFound();
            }

            var players = _uow.Players.GetByTeamId(id)
                .Select(MapPlayer)
                .ToList();

            return Ok(players);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateTeamRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            var team = new Team
            {
                Name = request.Name,
                City = request.City
            };

            _uow.Teams.Add(team);
            _uow.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = team.Id }, MapTeam(team));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateTeamRequest request)
        {
            var team = _uow.Teams.GetById(id);
            if (team == null)
            {
                return NotFound();
            }

            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            team.Name = request.Name;
            team.City = request.City;

            _uow.Teams.Update(team);
            _uow.SaveChanges();

            return Ok(MapTeam(team));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var team = _uow.Teams.GetById(id);
            if (team == null)
            {
                return NotFound();
            }

            var hasPlayers = _uow.Players.Find(p => p.TeamId == id).Any();
            var hasCoaches = _uow.Coaches.Find(c => c.TeamId == id).Any();
            var hasMatches = _uow.Matches.Find(m => m.HomeTeamId == id || m.AwayTeamId == id).Any();

            if (hasPlayers || hasCoaches || hasMatches)
            {
                return BadRequest("Tim ne moze da se obrise dok ima igrace, trenere ili utakmice.");
            }

            _uow.Teams.Remove(team);
            _uow.SaveChanges();

            return NoContent();
        }

        private IActionResult ValidationError(ValidationResult validationResult) =>
            BadRequest(validationResult.Errors.Select(error => new { error.PropertyName, error.ErrorMessage }));

        private static TeamDto MapTeam(Team team) =>
            new()
            {
                Id = team.Id,
                Name = team.Name,
                City = team.City
            };

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
    }
}
