namespace RankingPadelAPI.Domain
{
  public class Partido
  {
    public int Id { get; set; }

    public int Equipo1Id { get; set; }
    public Equipo Equipo1 { get; set; }   

    public int Equipo2Id { get; set; }
    public Equipo Equipo2 { get; set; }

    public int PuntosEquipo1 { get; set; }
    public int PuntosEquipo2 { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;
  }
}