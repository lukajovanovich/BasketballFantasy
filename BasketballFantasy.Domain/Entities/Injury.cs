namespace BasketballFantasy.Domain.Entities
{
    public class Injury
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player? Player { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime InjuryDate { get; set; }
        public DateTime? RecoveryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
