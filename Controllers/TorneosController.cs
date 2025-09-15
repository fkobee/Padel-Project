using Microsoft.AspNetCore.Mvc;
using RankingPadelAPI.Services;
using RankingPadelAPI.DTOs;

namespace RankingPadelAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TorneosController : ControllerBase
{
    private readonly ITorneosService _torneosService;

    public TorneosController(ITorneosService torneosService)
    {
        _torneosService = torneosService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TorneoDto>>> GetTorneos()
    {
        var torneos = await _torneosService.GetAllAsync();
        var dto = torneos.Select(t => new TorneoDto
        {
            Id = t.Id,
            Nombre = t.Nombre,
            Fecha = t.Fecha,
            Ubicacion = t.Ubicacion
        });
        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TorneoDto>> GetTorneo(int id)
    {
        var torneo = await _torneosService.GetByIdAsync(id);
        if (torneo == null) return NotFound();

        return Ok(new TorneoDto
        {
            Id = torneo.Id,
            Nombre = torneo.Nombre,
            Fecha = torneo.Fecha,
            Ubicacion = torneo.Ubicacion
        });
    }

    [HttpPost]
    public async Task<ActionResult<TorneoDto>> CreateTorneo([FromBody] CrearTorneoDto dto)
    {
        var torneo = new Models.Torneo
        {
            Nombre = dto.Nombre,
            Fecha = dto.Fecha,
            Ubicacion = dto.Ubicacion
        };

        var creado = await _torneosService.CreateAsync(torneo);

        return CreatedAtAction(nameof(GetTorneo), new { id = creado.Id }, new TorneoDto
        {
            Id = creado.Id,
            Nombre = creado.Nombre,
            Fecha = creado.Fecha,
            Ubicacion = creado.Ubicacion
        });
    }
}
