using System.Collections.Generic;
using RankingPadelAPI.Domain;

namespace RankingPadelAPI.Services
{
  public interface IPartidoService
  {
    IEnumerable<Partido> GetAll();
    Partido GetById(int id);
    void Add(Partido partido);
    void Update(Partido partido);
    void Delete(int id);
  }
}