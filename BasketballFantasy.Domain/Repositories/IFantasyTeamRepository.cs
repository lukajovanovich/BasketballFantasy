using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.Domain.Repositories
{
    public interface IFantasyTeamRepository : IRepository<FantasyTeam>
    {
        IEnumerable<FantasyTeam> GetAllDetailed();
        FantasyTeam? GetByIdWithDetails(int id);
    }
}
