namespace BasketballFantasy.API.DTOs.PlayerStatistics
{
    public class PlayerStatisticDto
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int MatchId { get; set; }
        public DateTime MatchDate { get; set; }
        public int Points { get; set; }
        public int Assists { get; set; }
        public int Rebounds { get; set; }
        public int Minutes { get; set; }
        public int Steals { get; set; }
        public int Blocks { get; set; }
        public int Turnovers { get; set; }
        public decimal FantasyPoints { get; set; }
    }

    public class CreatePlayerStatisticRequest
    {
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
        public int Points { get; set; }
        public int Assists { get; set; }
        public int Rebounds { get; set; }
        public int Minutes { get; set; }
        public int Steals { get; set; }
        public int Blocks { get; set; }
        public int Turnovers { get; set; }
    }

    public class UpdatePlayerStatisticRequest
    {
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
        public int Points { get; set; }
        public int Assists { get; set; }
        public int Rebounds { get; set; }
        public int Minutes { get; set; }
        public int Steals { get; set; }
        public int Blocks { get; set; }
        public int Turnovers { get; set; }
    }

    public class TopScorerDto
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public int GamesPlayed { get; set; }
        public int TotalPoints { get; set; }
    }
}
