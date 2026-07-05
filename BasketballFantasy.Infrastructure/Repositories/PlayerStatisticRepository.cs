using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BasketballFantasy.Infrastructure.Repositories
{
    public class PlayerStatisticRepository : Repository<PlayerStatistic>, IPlayerStatisticRepository
    {
        public PlayerStatisticRepository(BasketballFantasyContext context) : base(context)
        {
        }

        public IEnumerable<PlayerStatistic> GetAllDetailed() =>
            DbSet.Include(ps => ps.Player!)
                    .ThenInclude(p => p.Team!)
                .Include(ps => ps.Match!)
                    .ThenInclude(m => m.HomeTeam!)
                .Include(ps => ps.Match!)
                    .ThenInclude(m => m.AwayTeam!)
                .ToList();

        public PlayerStatistic? GetByIdDetailed(int id) =>
            DbSet.Include(ps => ps.Player!)
                    .ThenInclude(p => p.Team!)
                .Include(ps => ps.Match!)
                    .ThenInclude(m => m.HomeTeam!)
                .Include(ps => ps.Match!)
                    .ThenInclude(m => m.AwayTeam!)
                .FirstOrDefault(ps => ps.Id == id);

        public IEnumerable<PlayerStatistic> GetByPlayerId(int playerId) =>
            DbSet.Where(ps => ps.PlayerId == playerId)
                .Include(ps => ps.Player!)
                    .ThenInclude(p => p.Team!)
                .Include(ps => ps.Match!)
                    .ThenInclude(m => m.HomeTeam!)
                .Include(ps => ps.Match!)
                    .ThenInclude(m => m.AwayTeam!)
                .ToList();
    }
}
