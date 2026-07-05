namespace BasketballFantasy.Domain.Entities
{
    public class Coach
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public int TeamId { get; set; }
        public Team? Team { get; set; }

        public List<FantasyTeam> FantasyTeams { get; set; } = new();
    }
}
