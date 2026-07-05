using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BasketballFantasy.Infrastructure.Repositories
{
    public class InjuryRepository : Repository<Injury>, IInjuryRepository
    {
        public InjuryRepository(BasketballFantasyContext context) : base(context)
        {
        }

        public IEnumerable<Injury> GetAllWithPlayer() =>
            DbSet.Include(i => i.Player!)
                    .ThenInclude(p => p.Team!)
                .ToList();

        public Injury? GetByIdWithPlayer(int id) =>
            DbSet.Include(i => i.Player!)
                    .ThenInclude(p => p.Team!)
                .FirstOrDefault(i => i.Id == id);

        public IEnumerable<Injury> GetActiveInjuries() =>
            DbSet.Where(i => i.IsActive)
                .Include(i => i.Player!)
                    .ThenInclude(p => p.Team!)
                .ToList();
    }
}
