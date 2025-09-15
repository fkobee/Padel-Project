namespace RankingPadelAPI.Domain.Arguments
{
  public class RegisterPartidoArgs
  {
  public DateTime Fecha { get; set; }
  public int Equipo1Id { get; set; }
  public int Equipo2Id { get; set; }
  public int PuntuacionEquipo1 { get; set; }
  public int PuntuacionEquipo2 { get; set; }
  }
}