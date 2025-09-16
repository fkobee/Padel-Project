using RankingPadelAPI.Domain;
using RankingPadelAPI.Repositories;

namespace RankingPadelAPI.Services
{
    public class TorneoService : ITorneosService
    {
        private readonly ITorneoRepository _torneosRepository;

        public TorneoService(ITorneoRepository torneosRepository)
        {
            _torneosRepository = torneosRepository;
        }
        public IEnumerable<Torneo> GetAllTorneos()
        {
            return _torneosRepository.GetAllAsync().Result;
        }

        public Torneo GetTorneoById(int id)
        {
            var torneo = _torneosRepository.GetByIdAsync(id).Result;
            if (torneo == null)
            {
                throw new KeyNotFoundException($"No se encontró un torneo con el id {id}.");
            }
            return torneo;
        }

        public void AddTorneo(Torneo torneo)
        {
            _torneosRepository.AddAsync(torneo);
        }

        public void UpdateTorneo(Torneo torneo)
        {
            _torneosRepository.UpdateAsync(torneo);
        }

        public void DeleteTorneo(int id)
        {
            _torneosRepository.DeleteAsync(id);
        }
        
    }
}
