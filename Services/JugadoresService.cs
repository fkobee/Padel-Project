using System.Collections.Generic;
using System.Threading.Tasks;
using RankingPadelAPI.Models;
using Services;

namespace RankingPadelAPI.Services
{
  public class JugadoresService : IJugadoresService
  {
    private readonly ApplicationDbContext _context;

    public JugadoresService(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<List<Jugador>> GetAllAsync()
    {
      return await _context.Jugadores.ToListAsync();
    }

    public async Task<Jugador?> GetByIdAsync(int id)
    {
      return await _context.Jugadores.FindAsync(id);
    }

    public async Task<Jugador> AddAsync(Jugador jugador)
    {
      _context.Jugadores.Add(jugador);
      await _context.SaveChangesAsync();
      return jugador;
    }

    public async Task<bool> UpdateAsync(Jugador jugador)
    {
      var existing = await _context.Jugadores.FindAsync(jugador.Id);
      if (existing == null) return false;

      _context.Entry(existing).CurrentValues.SetValues(jugador);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
      var jugador = await _context.Jugadores.FindAsync(id);
      if (jugador == null) return false;

      _context.Jugadores.Remove(jugador);
      await _context.SaveChangesAsync();
      return true;
    }
  }
}