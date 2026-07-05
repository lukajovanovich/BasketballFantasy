using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.API.Services
{
    public class FantasyPointsService
    {
        public decimal CalculateFantasyPoints(PlayerStatistic statistic)
        {
            return statistic.Points
                + statistic.Rebounds * 1.2m
                + statistic.Assists * 1.5m
                + statistic.Steals * 2m
                + statistic.Blocks * 2m
                - statistic.Turnovers;
        }

        public decimal CalculateTotalFantasyPoints(IEnumerable<PlayerStatistic> statistics)
        {
            var total = statistics.Sum(CalculateFantasyPoints);
            return Math.Round(total, 2);
        }
    }
}
