using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.Domain.Repositories
{
    public interface ICoachRepository : IRepository<Coach>
    {
        IEnumerable<Coach> GetAllWithTeam();
        Coach? GetByIdWithTeam(int id);
    }
}
