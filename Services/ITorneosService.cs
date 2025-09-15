using System.Collections.Generic;
using RankingPadelAPI.Models;

namespace RankingPadelAPI.Services
{
  public interface ITorneosService
  {
    IEnumerable<Torneo> GetAllTorneos();
    Torneo GetTorneoById(int id);
    void AddTorneo(Torneo torneo);
    void UpdateTorneo(int id, Torneo torneo);
    void DeleteTorneo(int id);
  }
}