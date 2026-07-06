using AutoMapper;
using BasketballFantasy.API.DTOs.Coaches;
using BasketballFantasy.API.DTOs.FantasyTeams;
using BasketballFantasy.API.DTOs.Injuries;
using BasketballFantasy.API.DTOs.Matches;
using BasketballFantasy.API.DTOs.Players;
using BasketballFantasy.API.DTOs.PlayerStatistics;
using BasketballFantasy.API.DTOs.Teams;
using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Team, TeamDto>();
            CreateMap<CreateTeamRequest, Team>();
            CreateMap<UpdateTeamRequest, Team>();

            CreateMap<Player, PlayerDto>()
                .ForMember(destination => destination.TeamName, options => options.MapFrom(source => source.Team != null ? source.Team.Name : string.Empty));
            CreateMap<CreatePlayerRequest, Player>();
            CreateMap<UpdatePlayerRequest, Player>();

            CreateMap<Coach, CoachDto>()
                .ForMember(destination => destination.TeamName, options => options.MapFrom(source => source.Team != null ? source.Team.Name : string.Empty));
            CreateMap<CreateCoachRequest, Coach>();
            CreateMap<UpdateCoachRequest, Coach>();

            CreateMap<Match, MatchDto>()
                .ForMember(destination => destination.HomeTeamName, options => options.MapFrom(source => source.HomeTeam != null ? source.HomeTeam.Name : string.Empty))
                .ForMember(destination => destination.AwayTeamName, options => options.MapFrom(source => source.AwayTeam != null ? source.AwayTeam.Name : string.Empty));
            CreateMap<CreateMatchRequest, Match>();
            CreateMap<UpdateMatchRequest, Match>();

            CreateMap<PlayerStatistic, PlayerStatisticDto>()
                .ForMember(destination => destination.PlayerName, options => options.MapFrom(source => $"{source.Player!.FirstName} {source.Player.LastName}".Trim()))
                .ForMember(destination => destination.MatchDate, options => options.MapFrom(source => source.Match != null ? source.Match.MatchDate : DateTime.MinValue))
                .ForMember(destination => destination.FantasyPoints, options => options.MapFrom<PlayerStatisticFantasyPointsResolver>());
            CreateMap<CreatePlayerStatisticRequest, PlayerStatistic>();
            CreateMap<UpdatePlayerStatisticRequest, PlayerStatistic>();

            CreateMap<Injury, InjuryDto>()
                .ForMember(destination => destination.PlayerName, options => options.MapFrom(source => $"{source.Player!.FirstName} {source.Player.LastName}".Trim()));
            CreateMap<CreateInjuryRequest, Injury>();
            CreateMap<UpdateInjuryRequest, Injury>();

            CreateMap<FantasyTeam, FantasyTeamDto>()
                .ForMember(destination => destination.CoachName, options => options.MapFrom(source => $"{source.Coach!.FirstName} {source.Coach.LastName}".Trim()))
                .ForMember(destination => destination.StarterCount, options => options.MapFrom(source => source.FantasyTeamPlayers.Count(player => player.Role == FantasyRole.Starter)))
                .ForMember(destination => destination.BenchCount, options => options.MapFrom(source => source.FantasyTeamPlayers.Count(player => player.Role == FantasyRole.Bench)));
            CreateMap<FantasyTeamPlayer, FantasyTeamPlayerDto>()
                .ForMember(destination => destination.PlayerName, options => options.MapFrom(source => $"{source.Player!.FirstName} {source.Player.LastName}".Trim()))
                .ForMember(destination => destination.TeamName, options => options.MapFrom(source => source.Player != null && source.Player.Team != null ? source.Player.Team.Name : string.Empty))
                .ForMember(destination => destination.Role, options => options.MapFrom(source => source.Role.ToString()))
                .ForMember(destination => destination.TotalFantasyPoints, options => options.MapFrom<FantasyTeamPlayerTotalFantasyPointsResolver>());
            CreateMap<FantasyTeam, FantasyTeamDetailsDto>()
                .ForMember(destination => destination.CoachName, options => options.MapFrom(source => $"{source.Coach!.FirstName} {source.Coach.LastName}".Trim()))
                .ForMember(
                    destination => destination.Players,
                    options => options.MapFrom(source => source.FantasyTeamPlayers.OrderBy(player => player.Role).ThenBy(player => player.Player!.LastName)));
            CreateMap<CreateFantasyTeamRequest, FantasyTeam>();
            CreateMap<UpdateFantasyTeamRequest, FantasyTeam>();
        }
    }
}
