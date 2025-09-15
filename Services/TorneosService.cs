using RankingPadelAPI.Models;
using RankingPadelAPI.Repositories;

namespace RankingPadelAPI.Services
{
    public class TorneosService : ITorneosService
    {
        private readonly ITorneosRepository _torneosRepository;

        public TorneosService(ITorneosRepository torneosRepository)
        {
            _torneosRepository = torneosRepository;
        }

        public async Task<IEnumerable<Torneo>> GetAllAsync()
        {
            return await _torneosRepository.GetAllAsync();
        }

        public async Task<Torneo?> GetByIdAsync(int id)
        {
            return await _torneosRepository.GetByIdAsync(id);
        }

        public async Task<Torneo> CreateAsync(Torneo torneo)
        {
            return await _torneosRepository.CreateAsync(torneo);
        }

        public async Task<bool> UpdateAsync(int id, Torneo torneo)
        {
            return await _torneosRepository.UpdateAsync(id, torneo);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _torneosRepository.DeleteAsync(id);
        }
    }
}
