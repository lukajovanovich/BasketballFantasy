namespace BasketballFantasy.Domain.Entities
{
    public class PlayerStatistic
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player? Player { get; set; }
        public int MatchId { get; set; }
        public Match? Match { get; set; }
        public int Points { get; set; }
        public int Assists { get; set; }
        public int Rebounds { get; set; }
        public int Minutes { get; set; }
        public int Steals { get; set; }
        public int Blocks { get; set; }
        public int Turnovers { get; set; }
    }
}
