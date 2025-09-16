namespace RankingPadelAPI.Domain.Arguments;
public class PartidoDto
{
    public int Id { get; set; }
    public EquipoDto Equipo1 { get; set; }
    public EquipoDto Equipo2 { get; set; }
    public int PuntosEquipo1 { get; set; }
    public int PuntosEquipo2 { get; set; }
    public DateTime Fecha { get; set; }
}
