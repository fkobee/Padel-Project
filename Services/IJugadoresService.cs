using RankingPadelAPI.Models;

namespace RankingPadelAPI.Services
{
    public interface IJugadorService
    {
        Task<IEnumerable<Jugador>> GetJugadoresAsync();
        Task<Jugador> AddJugadorAsync(CreateJugadorArgs args);
        Task RegistrarPartidoAsync(RegisterPartidoArgs args);
    }
}
