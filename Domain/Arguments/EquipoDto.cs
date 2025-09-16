namespace RankingPadelAPI.Domain.Arguments;

public class EquipoDto
{
  public int Id { get; set; }
  public List<JugadorDto> Jugadores { get; set; } = new List<JugadorDto>();
}
