namespace BasketballFantasy.Domain.Entities
{
    public class FantasyTeamPlayer
    {
        public int Id { get; set; }
        public int FantasyTeamId { get; set; }
        public FantasyTeam? FantasyTeam { get; set; }
        public int PlayerId { get; set; }
        public Player? Player { get; set; }
        public FantasyRole Role { get; set; }
    }
}
