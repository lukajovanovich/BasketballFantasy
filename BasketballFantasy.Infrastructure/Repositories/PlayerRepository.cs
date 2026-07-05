using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BasketballFantasy.Infrastructure.Repositories
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        public PlayerRepository(BasketballFantasyContext context) : base(context)
        {
        }

        public IEnumerable<Player> GetAllWithTeam() =>
            DbSet.Include(p => p.Team).ToList();

        public Player? GetByIdWithTeam(int id) =>
            DbSet.Include(p => p.Team)
                .FirstOrDefault(p => p.Id == id);

        public Player? GetByIdWithStatistics(int id) =>
            DbSet.Include(p => p.Team)
                .Include(p => p.PlayerStatistics)
                    .ThenInclude(ps => ps.Match)
                .FirstOrDefault(p => p.Id == id);

        public IEnumerable<Player> GetByTeamId(int teamId) =>
            DbSet.Where(p => p.TeamId == teamId)
                .Include(p => p.Team)
                .ToList();
    }
}
