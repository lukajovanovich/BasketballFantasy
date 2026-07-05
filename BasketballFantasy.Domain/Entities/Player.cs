namespace BasketballFantasy.Domain.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Height { get; set; }
        public int JerseyNumber { get; set; }
        public int TeamId { get; set; }
        public Team? Team { get; set; }

        public List<PlayerStatistic> PlayerStatistics { get; set; } = new();
        public List<Injury> Injuries { get; set; } = new();
        public List<FantasyTeamPlayer> FantasyTeamPlayers { get; set; } = new();
    }
}
