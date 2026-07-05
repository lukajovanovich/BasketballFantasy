using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.Domain.Repositories
{
    public interface ITeamRepository : IRepository<Team>
    {
        Team? GetByIdWithDetails(int id);
    }
}
