using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using BasketballFantasy.Infrastructure.Repositories;

namespace BasketballFantasy.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BasketballFantasyContext _context;

        private ITeamRepository? _teams;
        private IPlayerRepository? _players;
        private ICoachRepository? _coaches;
        private IMatchRepository? _matches;
        private IPlayerStatisticRepository? _playerStatistics;
        private IInjuryRepository? _injuries;
        private IFantasyTeamRepository? _fantasyTeams;
        private IRepository<FantasyTeamPlayer>? _fantasyTeamPlayers;

        public UnitOfWork(BasketballFantasyContext context)
        {
            _context = context;
        }

        public ITeamRepository Teams =>
            _teams ??= new TeamRepository(_context);

        public IPlayerRepository Players =>
            _players ??= new PlayerRepository(_context);

        public ICoachRepository Coaches =>
            _coaches ??= new CoachRepository(_context);

        public IMatchRepository Matches =>
            _matches ??= new MatchRepository(_context);

        public IPlayerStatisticRepository PlayerStatistics =>
            _playerStatistics ??= new PlayerStatisticRepository(_context);

        public IInjuryRepository Injuries =>
            _injuries ??= new InjuryRepository(_context);

        public IFantasyTeamRepository FantasyTeams =>
            _fantasyTeams ??= new FantasyTeamRepository(_context);

        public IRepository<FantasyTeamPlayer> FantasyTeamPlayers =>
            _fantasyTeamPlayers ??= new Repository<FantasyTeamPlayer>(_context);

        public int SaveChanges() => _context.SaveChanges();

        public void Dispose() => _context.Dispose();
    }
}
