using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BasketballFantasy.Infrastructure.Repositories
{
    public class CoachRepository : Repository<Coach>, ICoachRepository
    {
        public CoachRepository(BasketballFantasyContext context) : base(context)
        {
        }

        public IEnumerable<Coach> GetAllWithTeam() =>
            DbSet.Include(c => c.Team).ToList();

        public Coach? GetByIdWithTeam(int id) =>
            DbSet.Include(c => c.Team)
                .FirstOrDefault(c => c.Id == id);
    }
}
