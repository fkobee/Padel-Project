using Microsoft.AspNetCore.Mvc;
using RankingPadelAPI.Services;
using RankingPadelAPI.Domain.Arguments;

namespace RankingPadelAPI.Controllers;

[ApiController]
[Route("jugadores")]
public class JugadoresController : ControllerBase
{
    private readonly IJugadorService _jugadorService;

    public JugadoresController(IJugadorService jugadorService)
    {
        _jugadorService = jugadorService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JugadorDto>>> Get()
    {
        var jugadores = await _jugadorService.GetJugadoresAsync();
        var dto = jugadores.Select(j => new JugadorDto
        {
            Id = j.Id,
            Nombre = j.Nombre,
            Puntos = j.Puntos
        });
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<JugadorDto>> Post([FromBody] CreateJugadorArgs dto)
    {
        var jugador = new CreateJugadorArgs(dto.Nombre, dto.FotoUrl, dto.Pala);

        var nuevoJugador = await _jugadorService.AddJugadorAsync(jugador);

        return Ok(new JugadorDto
        {
            Id = nuevoJugador.Id,
            Nombre = nuevoJugador.Nombre,
            Puntos = nuevoJugador.Puntos
        });
    }

    [HttpPost("partido")]
    public async Task<ActionResult> RegistrarPartido(RegisterPartidoArgs args)
    {
        try
        {
            await _jugadorService.RegistrarPartidoAsync(args);
            return Ok("Partido registrado");
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}
