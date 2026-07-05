using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ITeamRepository Teams { get; }
        IPlayerRepository Players { get; }
        ICoachRepository Coaches { get; }
        IMatchRepository Matches { get; }
        IPlayerStatisticRepository PlayerStatistics { get; }
        IInjuryRepository Injuries { get; }
        IFantasyTeamRepository FantasyTeams { get; }
        IRepository<FantasyTeamPlayer> FantasyTeamPlayers { get; }

        int SaveChanges();
    }
}
