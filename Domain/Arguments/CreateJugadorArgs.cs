namespace RankingPadelAPI.Domain.Arguments;

public sealed record class CreateJugadorArgs
{
  public string Nombre { get; set; } = string.Empty;

  public string Pala { get; set; } = string.Empty;

  public CreateJugadorArgs(string nombre, string pala)
  {
    Nombre = nombre;
    Pala = pala;
  }
}