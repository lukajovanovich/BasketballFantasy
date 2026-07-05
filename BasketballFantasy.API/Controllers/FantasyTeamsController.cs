using BasketballFantasy.API.DTOs.FantasyTeams;
using BasketballFantasy.API.Services;
using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace BasketballFantasy.API.Controllers
{
    [ApiController]
    [Route("fantasy-teams")]
    public class FantasyTeamsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateFantasyTeamRequest> _createValidator;
        private readonly IValidator<UpdateFantasyTeamRequest> _updateValidator;
        private readonly IValidator<AddFantasyTeamPlayerRequest> _addPlayerValidator;
        private readonly FantasyPointsService _fantasyPointsService;
        private readonly ExcelExportService _excelExportService;

        public FantasyTeamsController(
            IUnitOfWork uow,
            IValidator<CreateFantasyTeamRequest> createValidator,
            IValidator<UpdateFantasyTeamRequest> updateValidator,
            IValidator<AddFantasyTeamPlayerRequest> addPlayerValidator,
            FantasyPointsService fantasyPointsService,
            ExcelExportService excelExportService)
        {
            _uow = uow;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _addPlayerValidator = addPlayerValidator;
            _fantasyPointsService = fantasyPointsService;
            _excelExportService = excelExportService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var fantasyTeams = _uow.FantasyTeams.GetAllDetailed()
                .Select(MapFantasyTeam)
                .ToList();

            return Ok(fantasyTeams);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var fantasyTeam = _uow.FantasyTeams.GetByIdWithDetails(id);
            if (fantasyTeam == null)
            {
                return NotFound();
            }

            return Ok(MapFantasyTeamDetails(fantasyTeam));
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateFantasyTeamRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            if (_uow.Coaches.GetById(request.CoachId) == null)
            {
                return BadRequest($"Trener sa Id={request.CoachId} ne postoji.");
            }

            var fantasyTeam = new FantasyTeam
            {
                Name = request.Name,
                CoachId = request.CoachId
            };

            _uow.FantasyTeams.Add(fantasyTeam);
            _uow.SaveChanges();

            var createdFantasyTeam = _uow.FantasyTeams.GetByIdWithDetails(fantasyTeam.Id)!;
            return CreatedAtAction(nameof(GetById), new { id = fantasyTeam.Id }, MapFantasyTeamDetails(createdFantasyTeam));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateFantasyTeamRequest request)
        {
            var fantasyTeam = _uow.FantasyTeams.GetById(id);
            if (fantasyTeam == null)
            {
                return NotFound();
            }

            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            if (_uow.Coaches.GetById(request.CoachId) == null)
            {
                return BadRequest($"Trener sa Id={request.CoachId} ne postoji.");
            }

            fantasyTeam.Name = request.Name;
            fantasyTeam.CoachId = request.CoachId;

            _uow.FantasyTeams.Update(fantasyTeam);
            _uow.SaveChanges();

            var updatedFantasyTeam = _uow.FantasyTeams.GetByIdWithDetails(id)!;
            return Ok(MapFantasyTeamDetails(updatedFantasyTeam));
        }

        [HttpPost("{id}/players")]
        public IActionResult AddPlayer(int id, [FromBody] AddFantasyTeamPlayerRequest request)
        {
            var validationResult = _addPlayerValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ValidationError(validationResult);
            }

            if (_uow.FantasyTeams.GetById(id) == null)
            {
                return NotFound();
            }

            if (_uow.Players.GetById(request.PlayerId) == null)
            {
                return BadRequest($"Igrac sa Id={request.PlayerId} ne postoji.");
            }

            if (_uow.FantasyTeamPlayers.Find(ftp => ftp.FantasyTeamId == id && ftp.PlayerId == request.PlayerId).Any())
            {
                return BadRequest("Isti igrac ne moze biti dodat dva puta u isti fantasy tim.");
            }

            var starterCount = _uow.FantasyTeamPlayers.Find(ftp => ftp.FantasyTeamId == id && ftp.Role == FantasyRole.Starter).Count();
            var benchCount = _uow.FantasyTeamPlayers.Find(ftp => ftp.FantasyTeamId == id && ftp.Role == FantasyRole.Bench).Count();

            if (request.Role == FantasyRole.Starter && starterCount >= 5)
            {
                return BadRequest("Fantasy tim ne moze imati vise od 5 startera.");
            }

            if (request.Role == FantasyRole.Bench && benchCount >= 4)
            {
                return BadRequest("Fantasy tim ne moze imati vise od 4 igraca na klupi.");
            }

            var fantasyTeamPlayer = new FantasyTeamPlayer
            {
                FantasyTeamId = id,
                PlayerId = request.PlayerId,
                Role = request.Role
            };

            _uow.FantasyTeamPlayers.Add(fantasyTeamPlayer);
            _uow.SaveChanges();

            var updatedFantasyTeam = _uow.FantasyTeams.GetByIdWithDetails(id)!;
            return CreatedAtAction(nameof(GetById), new { id }, MapFantasyTeamDetails(updatedFantasyTeam));
        }

        [HttpGet("{id}/points")]
        public IActionResult GetPoints(int id)
        {
            var fantasyTeam = _uow.FantasyTeams.GetByIdWithDetails(id);
            if (fantasyTeam == null)
            {
                return NotFound();
            }

            var totalPoints = fantasyTeam.FantasyTeamPlayers.Sum(x =>
                _fantasyPointsService.CalculateTotalFantasyPoints(x.Player?.PlayerStatistics ?? new List<PlayerStatistic>()));

            var result = new FantasyTeamPointsDto
            {
                FantasyTeamId = fantasyTeam.Id,
                FantasyTeamName = fantasyTeam.Name,
                TotalFantasyPoints = Math.Round(totalPoints, 2)
            };

            return Ok(result);
        }

        [HttpGet("{id}/export")]
        public IActionResult Export(int id)
        {
            var fantasyTeam = _uow.FantasyTeams.GetByIdWithDetails(id);
            if (fantasyTeam == null)
            {
                return NotFound();
            }

            var content = _excelExportService.ExportFantasyTeamStatistics(fantasyTeam, _fantasyPointsService);
            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"fantasy-team-{id}.xlsx");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var fantasyTeam = _uow.FantasyTeams.GetById(id);
            if (fantasyTeam == null)
            {
                return NotFound();
            }

            _uow.FantasyTeams.Remove(fantasyTeam);
            _uow.SaveChanges();

            return NoContent();
        }

        private IActionResult ValidationError(ValidationResult validationResult) =>
            BadRequest(validationResult.Errors.Select(error => new { error.PropertyName, error.ErrorMessage }));

        private static FantasyTeamDto MapFantasyTeam(FantasyTeam fantasyTeam) =>
            new()
            {
                Id = fantasyTeam.Id,
                Name = fantasyTeam.Name,
                CoachId = fantasyTeam.CoachId,
                CoachName = $"{fantasyTeam.Coach?.FirstName} {fantasyTeam.Coach?.LastName}".Trim(),
                StarterCount = fantasyTeam.FantasyTeamPlayers.Count(x => x.Role == FantasyRole.Starter),
                BenchCount = fantasyTeam.FantasyTeamPlayers.Count(x => x.Role == FantasyRole.Bench)
            };

        private FantasyTeamDetailsDto MapFantasyTeamDetails(FantasyTeam fantasyTeam) =>
            new()
            {
                Id = fantasyTeam.Id,
                Name = fantasyTeam.Name,
                CoachId = fantasyTeam.CoachId,
                CoachName = $"{fantasyTeam.Coach?.FirstName} {fantasyTeam.Coach?.LastName}".Trim(),
                Players = fantasyTeam.FantasyTeamPlayers
                    .OrderBy(x => x.Role)
                    .ThenBy(x => x.Player!.LastName)
                    .Select(x => new FantasyTeamPlayerDto
                    {
                        Id = x.Id,
                        PlayerId = x.PlayerId,
                        PlayerName = $"{x.Player?.FirstName} {x.Player?.LastName}".Trim(),
                        TeamName = x.Player?.Team?.Name ?? string.Empty,
                        Role = x.Role.ToString(),
                        TotalFantasyPoints = _fantasyPointsService.CalculateTotalFantasyPoints(x.Player?.PlayerStatistics ?? new List<PlayerStatistic>())
                    })
                    .ToList()
            };
    }
}
