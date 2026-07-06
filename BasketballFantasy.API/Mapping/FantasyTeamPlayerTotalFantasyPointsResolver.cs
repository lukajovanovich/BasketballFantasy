using AutoMapper;
using BasketballFantasy.API.DTOs.FantasyTeams;
using BasketballFantasy.API.Services;
using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.API.Mapping
{
    public class FantasyTeamPlayerTotalFantasyPointsResolver : IValueResolver<FantasyTeamPlayer, FantasyTeamPlayerDto, decimal>
    {
        private readonly FantasyPointsService _fantasyPointsService;

        public FantasyTeamPlayerTotalFantasyPointsResolver(FantasyPointsService fantasyPointsService)
        {
            _fantasyPointsService = fantasyPointsService;
        }

        public decimal Resolve(
            FantasyTeamPlayer source,
            FantasyTeamPlayerDto destination,
            decimal destMember,
            ResolutionContext context) =>
            _fantasyPointsService.CalculateTotalFantasyPoints(source.Player?.PlayerStatistics ?? new List<PlayerStatistic>());
    }
}
