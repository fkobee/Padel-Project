using RankingPadelAPI.Domain;

namespace RankingPadelAPI.Repositories;

public interface IJugadorRepository
{
    Task<IEnumerable<Jugador>> GetAllAsync();
    Task<Jugador?> GetByIdAsync(int id);
    Task<Jugador> AddAsync(Jugador jugador);
    Task UpdateAsync(Jugador jugador);
    Task DeleteAsync(Jugador jugador);
}
