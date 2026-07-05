using BasketballFantasy.API.DTOs.Matches;
using BasketballFantasy.API.Services;
using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace BasketballFantasy.API.Controllers
{
    [ApiController]
    [Route("matches")]
    public class MatchesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateMatchRequest> _createValidator;
        private readonly IValidator<UpdateMatchRequest> _updateValidator;
        private readonly ExcelExportService _excelExportService;
        private readonly FantasyPointsService _fantasyPointsService;

        public MatchesController(
            IUnitOfWork uow,
            IValidator<CreateMatchRequest> createValidator,
            IValidator<UpdateMatchRequest> updateValidator,
            ExcelExportService excelExportService,
            FantasyPointsService fantasyPointsService)
        {
            _uow = uow;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _excelExportService = excelExportService;
            _fantasyPointsService = fantasyPointsService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var matches = _uow.Matches.GetAllWithTeams()
                .Select(MapMatch)
                .ToList();

            return Ok(matches);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var match = _uow.Matches.GetByIdWithDetails(id);
            if (match == null)
            {
                return NotFound();
            }

            return Ok(MapMatch(match));
        }

        [HttpGet("{id}/statistics/export")]
        public IActionResult ExportStatistics(int id)
        {
            var match = _uow.Matches.GetByIdWithStatistics(id);
            if (match == null)
            {
                return NotFound();
            }

            var content = _excelExportService.ExportMatchStatistics(match, _fantasyPointsService);
            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"match-statistics-{id}.xlsx");
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateMatchRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            if (_uow.Teams.GetById(request.HomeTeamId) == null || _uow.Teams.GetById(request.AwayTeamId) == null)
            {
                return BadRequest("Jedan ili oba tima ne postoje.");
            }

            var match = new Match
            {
                HomeTeamId = request.HomeTeamId,
                AwayTeamId = request.AwayTeamId,
                MatchDate = request.MatchDate,
                HomeScore = request.HomeScore,
                AwayScore = request.AwayScore
            };

            _uow.Matches.Add(match);
            _uow.SaveChanges();

            var createdMatch = _uow.Matches.GetByIdWithDetails(match.Id)!;
            return CreatedAtAction(nameof(GetById), new { id = match.Id }, MapMatch(createdMatch));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateMatchRequest request)
        {
            var match = _uow.Matches.GetById(id);
            if (match == null)
            {
                return NotFound();
            }

            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            if (_uow.Teams.GetById(request.HomeTeamId) == null || _uow.Teams.GetById(request.AwayTeamId) == null)
            {
                return BadRequest("Jedan ili oba tima ne postoje.");
            }

            match.HomeTeamId = request.HomeTeamId;
            match.AwayTeamId = request.AwayTeamId;
            match.MatchDate = request.MatchDate;
            match.HomeScore = request.HomeScore;
            match.AwayScore = request.AwayScore;

            _uow.Matches.Update(match);
            _uow.SaveChanges();

            var updatedMatch = _uow.Matches.GetByIdWithDetails(id)!;
            return Ok(MapMatch(updatedMatch));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var match = _uow.Matches.GetById(id);
            if (match == null)
            {
                return NotFound();
            }

            _uow.Matches.Remove(match);
            _uow.SaveChanges();

            return NoContent();
        }

        private IActionResult ValidationError(ValidationResult validationResult) =>
            BadRequest(validationResult.Errors.Select(error => new { error.PropertyName, error.ErrorMessage }));

        private static MatchDto MapMatch(Match match) =>
            new()
            {
                Id = match.Id,
                HomeTeamId = match.HomeTeamId,
                HomeTeamName = match.HomeTeam?.Name ?? string.Empty,
                AwayTeamId = match.AwayTeamId,
                AwayTeamName = match.AwayTeam?.Name ?? string.Empty,
                MatchDate = match.MatchDate,
                HomeScore = match.HomeScore,
                AwayScore = match.AwayScore
            };
    }
}
