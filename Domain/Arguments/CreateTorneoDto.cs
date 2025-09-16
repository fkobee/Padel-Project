namespace RankingPadelAPI.Domain.Arguments
{
  public class CreateTorneoDto
  {
    public string Nombre { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Ubicacion { get; set; }
  }
}