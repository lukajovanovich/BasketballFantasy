using BasketballFantasy.Domain.Entities;

namespace BasketballFantasy.Domain.Repositories
{
    public interface IPlayerStatisticRepository : IRepository<PlayerStatistic>
    {
        IEnumerable<PlayerStatistic> GetAllDetailed();
        PlayerStatistic? GetByIdDetailed(int id);
        IEnumerable<PlayerStatistic> GetByPlayerId(int playerId);
    }
}
