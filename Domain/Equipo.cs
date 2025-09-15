namespace RankingPadelAPI.Domain;
public class Equipo
{
  public int Id { get; set; }
  public List<Jugador> Jugadores { get; set; } = new List<Jugador>();
}
