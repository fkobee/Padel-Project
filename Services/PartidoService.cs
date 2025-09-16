using RankingPadelAPI.Domain;
using RankingPadelAPI.Repositories;

namespace RankingPadelAPI.Services;
  public class PartidoService : IPartidoService
  {
    private readonly IPartidoRepository _partidoRepository;

    public PartidoService(IPartidoRepository partidoRepository)
    {
      _partidoRepository = partidoRepository;
    }

    public IEnumerable<Partido> GetAll()
    {
      return _partidoRepository.GetAllAsync().Result;
    }

    public Partido GetById(int id)
    {
      return _partidoRepository.GetByIdAsync(id).Result;
    }

    public void Add(Partido partido)
    {
      _partidoRepository.AddAsync(partido);
    }

    public void Update(Partido partido)
    {
      _partidoRepository.UpdateAsync(partido);
    }

    public void Delete(int id)
    {
      _partidoRepository.DeleteAsync(id);
    }

}