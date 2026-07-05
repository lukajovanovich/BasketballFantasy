using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BasketballFantasy.Infrastructure.Repositories
{
    public class MatchRepository : Repository<Match>, IMatchRepository
    {
        public MatchRepository(BasketballFantasyContext context) : base(context)
        {
        }

        public IEnumerable<Match> GetAllWithTeams() =>
            DbSet.Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .ToList();

        public Match? GetByIdWithDetails(int id) =>
            DbSet.Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefault(m => m.Id == id);

        public Match? GetByIdWithStatistics(int id) =>
            DbSet.Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.PlayerStatistics)
                    .ThenInclude(ps => ps.Player!)
                        .ThenInclude(p => p.Team!)
                .FirstOrDefault(m => m.Id == id);
    }
}
