using RankingPadelAPI.Domain;
using RankingPadelAPI.Repositories;

namespace RankingPadelAPI.Services
{
  public class PartidoService
  {
    private readonly IPartidoRepository _partidoRepository;

    public PartidoService(IPartidoRepository partidoRepository)
    {
      _partidoRepository = partidoRepository;
    }

    public async Task<IEnumerable<Partido>> GetAllAsync()
    {
      return await _partidoRepository.GetAllAsync();
    }

    public async Task<Partido> GetByIdAsync(int id)
    {
      return await _partidoRepository.GetByIdAsync(id);
    }

    public async Task<Partido> CreateAsync(Partido partido)
    {
      await _partidoRepository.AddAsync(partido);
      return partido;
    }

    public async Task<bool> UpdateAsync(Partido partido)
    {
      await _partidoRepository.UpdateAsync(partido);
      return true;
    }

    public async Task DeleteAsync(int id)
    {
      await _partidoRepository.DeleteAsync(id);
    }
  }
}