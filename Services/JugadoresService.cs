using Microsoft.EntityFrameworkCore;
using RankingPadelAPI.Data;
using RankingPadelAPI.Models;

namespace RankingPadelAPI.Services
{
    public class JugadorService : IJugadorService
    {
        private readonly ApplicationDbContext _context;

        public JugadorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Jugador>> GetJugadoresAsync()
        {
            return await _context.Jugadores
                .OrderByDescending(j => j.Puntos)
                .ToListAsync();
        }

        public async Task<Jugador> AddJugadorAsync(Jugador jugador)
        {
            _context.Jugadores.Add(jugador);
            await _context.SaveChangesAsync();
            return jugador;
        }

        public async Task RegistrarPartidoAsync(int ganadorId, int perdedorId)
        {
            var ganador = await _context.Jugadores.FindAsync(ganadorId);
            var perdedor = await _context.Jugadores.FindAsync(perdedorId);

            if (ganador == null || perdedor == null)
                throw new Exception("Jugador no encontrado");

            ganador.Puntos += 3;
            
            await _context.SaveChangesAsync();
        }
    }
}
