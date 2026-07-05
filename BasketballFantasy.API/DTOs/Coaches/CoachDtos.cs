namespace BasketballFantasy.API.DTOs.Coaches
{
    public class CoachDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
    }

    public class CreateCoachRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public int TeamId { get; set; }
    }

    public class UpdateCoachRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public int TeamId { get; set; }
    }
}
