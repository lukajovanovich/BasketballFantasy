namespace BasketballFantasy.API.DTOs.Teams
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class CreateTeamRequest
    {
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class UpdateTeamRequest
    {
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
}
