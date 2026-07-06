using AutoMapper;
using BasketballFantasy.API.DTOs.PlayerStatistics;
using BasketballFantasy.API.Services;
using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.API.Mapping
{
    public class PlayerStatisticFantasyPointsResolver : IValueResolver<PlayerStatistic, PlayerStatisticDto, decimal>
    {
        private readonly FantasyPointsService _fantasyPointsService;

        public PlayerStatisticFantasyPointsResolver(FantasyPointsService fantasyPointsService)
        {
            _fantasyPointsService = fantasyPointsService;
        }

        public decimal Resolve(
            PlayerStatistic source,
            PlayerStatisticDto destination,
            decimal destMember,
            ResolutionContext context) =>
            _fantasyPointsService.CalculateFantasyPoints(source);
    }
}
