using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.Domain.Repositories
{
    public interface IMatchRepository : IRepository<Match>
    {
        IEnumerable<Match> GetAllWithTeams();
        Match? GetByIdWithDetails(int id);
        Match? GetByIdWithStatistics(int id);
    }
}
