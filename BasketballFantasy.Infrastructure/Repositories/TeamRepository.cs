using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BasketballFantasy.Infrastructure.Repositories
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        public TeamRepository(BasketballFantasyContext context) : base(context)
        {
        }

        public Team? GetByIdWithDetails(int id) =>
            DbSet.Include(t => t.Players)
                .Include(t => t.Coaches)
                .FirstOrDefault(t => t.Id == id);
    }
}
