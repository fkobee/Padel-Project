using Microsoft.AspNetCore.Mvc;
using RankingPadelAPI.Models;
using RankingPadelAPI.Services;

namespace RankingPadelAPI.Controllers;

[ApiController]
[Route("jugadores")]
public sealed class JugadoresController : ControllerBase
{
    private readonly IJugadorService _jugadorService;

    public JugadoresController(IJugadorService jugadorService)
    {
        _jugadorService = jugadorService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Jugador>>> Get()
    {
        var jugadores = await _jugadorService.GetJugadoresAsync();
        return Ok(jugadores);
    }

    [HttpPost]
    public async Task<ActionResult<Jugador>> Post(Jugador jugador)
    {
        var nuevoJugador = await _jugadorService.AddJugadorAsync(jugador);
        return Ok(nuevoJugador);
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