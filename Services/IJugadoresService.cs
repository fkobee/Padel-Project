namespace Services
{
  using RankingPadelAPI.Models;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  public interface IJugadoresService
  {
    Task<List<Jugador>> GetAllAsync();
    Task<Jugador?> GetByIdAsync(int id);
    Task<Jugador> AddAsync(Jugador jugador);
    Task<bool> UpdateAsync(Jugador jugador);
    Task<bool> DeleteAsync(int id);
  }
}