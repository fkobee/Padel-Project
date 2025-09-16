using RankingPadelAPI.Domain;

namespace RankingPadelAPI.Repositories
{
  public interface ITorneosRepository
  {
    Task<IEnumerable<Torneo>> GetAllAsync();
    Task<Torneo?> GetByIdAsync(int id);
    Task AddAsync(Torneo torneo);
    Task UpdateAsync(Torneo torneo);
    Task DeleteAsync(int id);
  }
}