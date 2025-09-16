using System.Collections.Generic;
using System.Threading.Tasks;
using RankingPadelAPI.Domain;

namespace RankingPadelAPI.Repositories
{
  public class TorneoRepository : ITorneoRepository
  {
    private readonly RankingPadelDbContext _context;

    public TorneoRepository(RankingPadelDbContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<Torneo>> GetAllAsync()
    {
      return await _context.Torneos.ToListAsync();
    }

    public async Task<Torneo> GetByIdAsync(int id)
    {
      return await _context.Torneos.FindAsync(id);
    }

    public async Task AddAsync(Torneo torneo)
    {
      _context.Torneos.Add(torneo);
      await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Torneo torneo)
    {
      _context.Torneos.Update(torneo);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
      var torneo = await _context.Torneos.FindAsync(id);
      if (torneo != null)
      {
        _context.Torneos.Remove(torneo);
        await _context.SaveChangesAsync();
      }
    }
  }
}