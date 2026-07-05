namespace BasketballFantasy.Domain.Entities
{
    public class FantasyTeam
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CoachId { get; set; }
        public Coach? Coach { get; set; }

        public List<FantasyTeamPlayer> FantasyTeamPlayers { get; set; } = new();
    }
}
