using System.Collections.Generic;
using RankingPadelAPI.Domain;

namespace RankingPadelAPI.Services
{
  public interface ITorneosService
  {
    IEnumerable<Torneo> GetAllTorneos();
    Torneo GetTorneoById(int id);
    void AddTorneo(Torneo torneo);
    void UpdateTorneo(Torneo torneo);
    void DeleteTorneo(int id);
  }
}