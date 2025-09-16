namespace RankingPadelAPI.Domain.Arguments;

public class CrearPartidoArgs
{
    public int Equipo1Id { get; set; }
    public int Equipo2Id { get; set; }
    public int PuntosEquipo1 { get; set; }
    public int PuntosEquipo2 { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
}
