using AutoMapper;
using BasketballFantasy.API.Common.Calculations;
using BasketballFantasy.API.DTOs.PlayerStatistics;
using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.API.Mapping
{
    public class PlayerStatisticFantasyPointsResolver : IValueResolver<PlayerStatistic, PlayerStatisticDto, decimal>
    {
        public decimal Resolve(
            PlayerStatistic source,
            PlayerStatisticDto destination,
            decimal destMember,
            ResolutionContext context) =>
            FantasyPointsCalculator.CalculateFantasyPoints(source);
    }
}
