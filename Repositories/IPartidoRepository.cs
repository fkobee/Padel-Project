using RankingPadelAPI.Domain;

namespace RankingPadelAPI.Repositories
{
  public interface IPartidoRepository
  {
    Task<IEnumerable<Partido>> GetAllAsync();
    Task<Partido> GetByIdAsync(int id);
    Task AddAsync(Partido partido);
    Task UpdateAsync(Partido partido);
    Task DeleteAsync(int id);
  }
}