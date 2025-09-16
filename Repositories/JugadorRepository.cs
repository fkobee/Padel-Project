using RankingPadelAPI.Domain;
namespace RankingPadelAPI.Repositories;
public class JugadorRepository : IJugadorRepository
{
    private readonly ApplicationDbContext _context;

    public JugadorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Jugador>> GetAllAsync()
        => await _context.Jugadores.OrderByDescending(j => j.Puntos).ToListAsync();

    public async Task<Jugador?> GetByIdAsync(int id)
        => await _context.Jugadores.FindAsync(id);

    public async Task<Jugador> AddAsync(Jugador jugador)
    {
        _context.Jugadores.Add(jugador);
        await _context.SaveChangesAsync();
        return jugador;
    }

    public async Task UpdateAsync(Jugador jugador)
    {
        _context.Entry(jugador).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Jugador jugador)
    {
        _context.Jugadores.Remove(jugador);
        await _context.SaveChangesAsync();
    }
}
