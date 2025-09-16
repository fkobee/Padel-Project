namespace RankingPadelAPI.Domain.Arguments
{
  public class RegisterPartidoArgs
  {
  public DateTime Fecha { get; set; }
  public int GanadorId { get; set; }
  public int PerdedorId { get; set; }
  public int PuntuacionGanador { get; set; }
  public int PuntuacionPerdedor { get; set; }
  }
}