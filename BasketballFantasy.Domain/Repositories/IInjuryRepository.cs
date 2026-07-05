using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.Domain.Repositories
{
    public interface IInjuryRepository : IRepository<Injury>
    {
        IEnumerable<Injury> GetAllWithPlayer();
        Injury? GetByIdWithPlayer(int id);
        IEnumerable<Injury> GetActiveInjuries();
    }
}
