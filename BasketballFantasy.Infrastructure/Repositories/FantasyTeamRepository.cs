using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BasketballFantasy.Infrastructure.Repositories
{
    public class FantasyTeamRepository : Repository<FantasyTeam>, IFantasyTeamRepository
    {
        public FantasyTeamRepository(BasketballFantasyContext context) : base(context)
        {
        }

        public IEnumerable<FantasyTeam> GetAllDetailed() =>
            DbSet.Include(ft => ft.Coach!)
                    .ThenInclude(c => c.Team!)
                .Include(ft => ft.FantasyTeamPlayers)
                    .ThenInclude(ftp => ftp.Player!)
                        .ThenInclude(p => p.Team!)
                .ToList();

        public FantasyTeam? GetByIdWithDetails(int id) =>
            DbSet.Include(ft => ft.Coach!)
                    .ThenInclude(c => c.Team!)
                .Include(ft => ft.FantasyTeamPlayers)
                    .ThenInclude(ftp => ftp.Player!)
                        .ThenInclude(p => p.Team!)
                .Include(ft => ft.FantasyTeamPlayers)
                    .ThenInclude(ftp => ftp.Player!)
                        .ThenInclude(p => p.PlayerStatistics)
                .FirstOrDefault(ft => ft.Id == id);
    }
}
