namespace BasketballFantasy.API.DTOs.Players
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Height { get; set; }
        public int JerseyNumber { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
    }

    public class CreatePlayerRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Height { get; set; }
        public int JerseyNumber { get; set; }
        public int TeamId { get; set; }
    }

    public class UpdatePlayerRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Height { get; set; }
        public int JerseyNumber { get; set; }
        public int TeamId { get; set; }
    }

    public class PlayerTotalsDto
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int GamesPlayed { get; set; }
        public int Points { get; set; }
        public int Assists { get; set; }
        public int Rebounds { get; set; }
        public int Minutes { get; set; }
        public int Steals { get; set; }
        public int Blocks { get; set; }
        public int Turnovers { get; set; }
        public decimal FantasyPoints { get; set; }
    }
}
