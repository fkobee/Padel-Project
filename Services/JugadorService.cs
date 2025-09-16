using RankingPadelAPI.Domain;
using RankingPadelAPI.Domain.Arguments;
using RankingPadelAPI.Repositories;

namespace RankingPadelAPI.Services;

public class JugadorService : IJugadorService
{
    private readonly IJugadorRepository _jugadorRepository;

    public JugadorService(IJugadorRepository jugadorRepository)
    {
        _jugadorRepository = jugadorRepository;
    }

    public async Task<IEnumerable<Jugador>> GetJugadoresAsync()
        => await _jugadorRepository.GetAllAsync();

    public async Task<Jugador> AddJugadorAsync(CreateJugadorArgs args)
    {
        var jugador = new Jugador
        {
            Nombre = args.Nombre,
            Pala = args.Pala,
            Foto = args.FotoUrl,
            Puntos = 0
        };

        await _jugadorRepository.AddAsync(jugador);
        return jugador;
    }

    public async Task RegistrarPartidoAsync(RegisterPartidoArgs args)
    {
        var ganador = await _jugadorRepository.GetByIdAsync(args.GanadorId);
        var perdedor = await _jugadorRepository.GetByIdAsync(args.PerdedorId);

        if (ganador == null || perdedor == null)
            throw new Exception("Jugador no encontrado");

        ganador.Puntos += 3;
        await _jugadorRepository.UpdateAsync(ganador);
    }
}
