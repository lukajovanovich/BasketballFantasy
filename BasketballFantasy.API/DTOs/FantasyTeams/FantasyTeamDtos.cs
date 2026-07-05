using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.API.DTOs.FantasyTeams
{
    public class FantasyTeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CoachId { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public int StarterCount { get; set; }
        public int BenchCount { get; set; }
    }

    public class FantasyTeamPlayerDto
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public decimal TotalFantasyPoints { get; set; }
    }

    public class FantasyTeamDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CoachId { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public List<FantasyTeamPlayerDto> Players { get; set; } = new();
    }

    public class FantasyTeamPointsDto
    {
        public int FantasyTeamId { get; set; }
        public string FantasyTeamName { get; set; } = string.Empty;
        public decimal TotalFantasyPoints { get; set; }
    }

    public class CreateFantasyTeamRequest
    {
        public string Name { get; set; } = string.Empty;
        public int CoachId { get; set; }
    }

    public class UpdateFantasyTeamRequest
    {
        public string Name { get; set; } = string.Empty;
        public int CoachId { get; set; }
    }

    public class AddFantasyTeamPlayerRequest
    {
        public int PlayerId { get; set; }
        public FantasyRole Role { get; set; }
    }
}
