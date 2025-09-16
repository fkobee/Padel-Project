namespace RankingPadelAPI.Domain.Arguments
{
  public class TorneoDto
  {
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Ubicacion { get; set; } = string.Empty;
  }
}