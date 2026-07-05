using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.Domain.Repositories
{
    public interface IPlayerRepository : IRepository<Player>
    {
        IEnumerable<Player> GetAllWithTeam();
        Player? GetByIdWithTeam(int id);
        Player? GetByIdWithStatistics(int id);
        IEnumerable<Player> GetByTeamId(int teamId);
    }
}
