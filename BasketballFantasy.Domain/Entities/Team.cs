namespace BasketballFantasy.Domain.Entities
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        public List<Player> Players { get; set; } = new();
        public List<Coach> Coaches { get; set; } = new();
        public List<Match> HomeMatches { get; set; } = new();
        public List<Match> AwayMatches { get; set; } = new();
    }
}
