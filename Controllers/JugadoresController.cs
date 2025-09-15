using Microsoft.AspNetCore.Mvc;
using RankingPadelAPI.Services;
using RankingPadelAPI.DTOs;

namespace RankingPadelAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<ActionResult<JugadorDto>> Post([FromBody] CrearJugadorDto dto)
    {
        var jugador = new Models.Jugador
        {
            Nombre = dto.Nombre,
            FotoUrl = dto.FotoUrl,
            Puntos = 0
        };

        var nuevoJugador = await _jugadorService.AddJugadorAsync(jugador);

        return Ok(new JugadorDto
        {
            Id = nuevoJugador.Id,
            Nombre = nuevoJugador.Nombre,
            Puntos = nuevoJugador.Puntos
        });
    }

    [HttpPost("partido")]
    public async Task<ActionResult> RegistrarPartido(int ganadorId, int perdedorId)
    {
        try
        {
            await _jugadorService.RegistrarPartidoAsync(ganadorId, perdedorId);
            return Ok("Partido registrado");
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}
