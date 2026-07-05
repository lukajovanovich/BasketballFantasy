namespace BasketballFantasy.API.DTOs.Injuries
{
    public class InjuryDto
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime InjuryDate { get; set; }
        public DateTime? RecoveryDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateInjuryRequest
    {
        public int PlayerId { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime InjuryDate { get; set; }
        public DateTime? RecoveryDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateInjuryRequest
    {
        public int PlayerId { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime InjuryDate { get; set; }
        public DateTime? RecoveryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
