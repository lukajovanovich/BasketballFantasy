using AutoMapper;
using BasketballFantasy.API.Common.Calculations;
using BasketballFantasy.API.DTOs.FantasyTeams;
using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.API.Mapping
{
    public class FantasyTeamPlayerTotalFantasyPointsResolver : IValueResolver<FantasyTeamPlayer, FantasyTeamPlayerDto, decimal>
    {
        public decimal Resolve(
            FantasyTeamPlayer source,
            FantasyTeamPlayerDto destination,
            decimal destMember,
            ResolutionContext context) =>
            FantasyPointsCalculator.CalculateTotalFantasyPoints(source.Player?.PlayerStatistics ?? new List<PlayerStatistic>());
    }
}
