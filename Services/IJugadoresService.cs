using RankingPadelAPI.Models;

namespace RankingPadelAPI.Services
{
    public interface IJugadorService
    {
        Task<IEnumerable<Jugador>> GetJugadoresAsync();
        Task<Jugador> AddJugadorAsync(Jugador jugador);
        Task RegistrarPartidoAsync(int ganadorId, int perdedorId);
    }
}
