namespace BasketballFantasy.API.DTOs.Matches
{
    public class MatchDto
    {
        public int Id { get; set; }
        public int HomeTeamId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public int AwayTeamId { get; set; }
        public string AwayTeamName { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
    }

    public class CreateMatchRequest
    {
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public DateTime MatchDate { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
    }

    public class UpdateMatchRequest
    {
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public DateTime MatchDate { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
    }
}
