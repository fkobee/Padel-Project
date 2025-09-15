using RankingPadelAPI.Domain;

namespace RankingPadelAPI.Models
{
  public class Torneo
  {
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Ubicacion { get; set; } = string.Empty;

    public List<Partido> Partidos { get; set; } = new List<Partido>();
    }
}