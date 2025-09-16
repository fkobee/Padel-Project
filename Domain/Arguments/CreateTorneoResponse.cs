namespace RankingPadelAPI.Domain.Arguments;

public sealed record class CreateTorneoResponse
{
  public string Id { get; set; } = string.Empty;
  public string Nombre { get; set; } = string.Empty;
  public DateTime FechaInicio { get; set; }

  public CreateTorneoResponse(string id, string nombre, DateTime fechaInicio)
  {
    Id = id;
    Nombre = nombre;
    FechaInicio = fechaInicio;
  }

}
